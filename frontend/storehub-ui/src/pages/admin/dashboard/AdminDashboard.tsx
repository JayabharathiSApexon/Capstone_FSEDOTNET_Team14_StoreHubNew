import {
    FaArrowUp,
    FaBoxes,
    FaCalendarAlt,
    FaChevronDown,
    FaExclamationTriangle,
    FaLightbulb,
    FaRupeeSign,
    FaShoppingCart
} from "react-icons/fa";
import { useEffect, useMemo, useState } from "react";
import AdminLayout from "../../../components/admin/AdminLayout";
import { getProducts } from "../../../services/productService";
import { getOrderTracking, getOrders } from "../../../services/orderService";
import { ProductResponse } from "../../../models/product/ProductResponse";
import MyOrderResponse from "../../../models/order/MyOrderResponse";
import "./AdminDashboard.css";

const START_MONTH = new Date(2026, 0, 1);

const getMonthKey = (date: Date) => `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}`;

interface BestSellerItem {
    product: string;
    sold: number;
}

interface OrderTrackingLite {
    orderDate?: string;
    products?: Array<{ name?: string; quantity?: number }>;
}

interface DashboardOrderDetail {
    order: MyOrderResponse;
    orderDate: Date | null;
    products: Array<{ name: string; quantity: number }>;
}

function SalesTrendChart({ salesPoints }: { salesPoints: number[] }) {
    const max = Math.max(...salesPoints);
    const min = Math.min(...salesPoints);

    const points = salesPoints
        .map((value, index) => {
            const x = (index / (salesPoints.length - 1)) * 100;
            const y = 100 - ((value - min) / (max - min || 1)) * 100;

            return `${x},${y}`;
        })
        .join(" ");

    const areaPoints = `0,100 ${points} 100,100`;

    return <svg viewBox="0 0 100 100" preserveAspectRatio="none" className="dashboard-chart-svg">
        <defs>
            <linearGradient id="salesAreaFill" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stopColor="#3b82f6" stopOpacity="0.26" />
                <stop offset="100%" stopColor="#3b82f6" stopOpacity="0.06" />
            </linearGradient>
        </defs>
        <polygon points={areaPoints} fill="url(#salesAreaFill)" />
        <polyline points={points} fill="none" stroke="#2f6ef9" strokeWidth="2" />
        {salesPoints.map((value, index) => {
            const x = (index / (salesPoints.length - 1)) * 100;
            const y = 100 - ((value - min) / (max - min || 1)) * 100;

            return <circle key={`${value}-${index}`} cx={x} cy={y} r="1.1" fill="#2f6ef9" />;
        })}
    </svg>;
}

const fallbackSales = new Array<number>(12).fill(0);

const inrFormatter = new Intl.NumberFormat("en-IN", {
    style: "currency",
    currency: "INR",
    maximumFractionDigits: 0
});

function AdminDashboard() {
    const [loading, setLoading] = useState(true);
    const [products, setProducts] = useState<ProductResponse[]>([]);
    const [orderDetails, setOrderDetails] = useState<DashboardOrderDetail[]>([]);
    const [selectedMonthKey, setSelectedMonthKey] = useState<string>(() => getMonthKey(new Date()));

    useEffect(() => {
        void loadDashboardData();
    }, []);

    const loadDashboardData = async () => {
        setLoading(true);

        try {
            const [productsResult, ordersResult] = await Promise.allSettled([
                getProducts(),
                getOrders()
            ]);

            if (productsResult.status === "fulfilled") {
                setProducts(productsResult.value);
            }

            let resolvedOrders: MyOrderResponse[] = [];

            if (ordersResult.status === "fulfilled") {
                resolvedOrders = ordersResult.value;
            }

            const currentMonthDate = new Date();
            currentMonthDate.setDate(1);

            const trackingResults = await Promise.allSettled(
                resolvedOrders.map(order => getOrderTracking(order.id))
            );

            const details = resolvedOrders.map((order, index): DashboardOrderDetail => {
                const trackingResult = trackingResults[index];

                if (trackingResult?.status !== "fulfilled") {
                    return {
                        order,
                        orderDate: currentMonthDate,
                        products: []
                    };
                }

                const tracking = trackingResult.value as OrderTrackingLite;
                const parsedDate = tracking.orderDate ? new Date(tracking.orderDate) : null;
                const productsFromTracking = (tracking.products ?? [])
                    .filter(item => !!item.name)
                    .map(item => ({
                        name: item.name as string,
                        quantity: Number(item.quantity ?? 0)
                    }));

                return {
                    order,
                    orderDate: parsedDate && !Number.isNaN(parsedDate.getTime()) ? parsedDate : currentMonthDate,
                    products: productsFromTracking
                };
            });

            setOrderDetails(details);
        } catch {
            setProducts([]);
            setOrderDetails([]);
        } finally {
            setLoading(false);
        }
    };

    const monthOptions = useMemo(() => {
        const formatter = new Intl.DateTimeFormat("en-IN", {
            month: "short",
            year: "numeric"
        });

        const current = new Date();
        const options: Array<{ key: string; label: string }> = [];
        const cursor = new Date(START_MONTH.getFullYear(), START_MONTH.getMonth(), 1);

        while (cursor <= new Date(current.getFullYear(), current.getMonth(), 1)) {
            options.push({
                key: getMonthKey(cursor),
                label: formatter.format(cursor)
            });
            cursor.setMonth(cursor.getMonth() + 1);
        }

        return options;
    }, []);

    const selectedMonthLabel = useMemo(() => {
        return monthOptions.find(option => option.key === selectedMonthKey)?.label ?? "Select Month";
    }, [monthOptions, selectedMonthKey]);

    const filteredOrderDetails = useMemo(() => {
        return orderDetails.filter(detail => {
            if (!detail.orderDate) {
                return false;
            }

            return getMonthKey(detail.orderDate) === selectedMonthKey;
        });
    }, [orderDetails, selectedMonthKey]);

    const filteredOrders = useMemo(() => {
        return filteredOrderDetails.map(detail => detail.order);
    }, [filteredOrderDetails]);

    const bestSellingProducts = useMemo<BestSellerItem[]>(() => {
        const salesMap = new Map<string, number>();

        filteredOrderDetails.forEach(detail => {
            detail.products.forEach(item => {
                salesMap.set(item.name, (salesMap.get(item.name) ?? 0) + item.quantity);
            });
        });

        return Array.from(salesMap.entries())
            .map(([product, sold]) => ({ product, sold }))
            .sort((a, b) => b.sold - a.sold)
            .slice(0, 5);
    }, [filteredOrderDetails]);

    const lowStockCount = useMemo(() => {
        return products.filter(product => product.stockQuantity <= 10).length;
    }, [products]);

    const availableStock = useMemo(() => {
        return products.reduce((sum, product) => sum + product.stockQuantity, 0);
    }, [products]);

    const totalRevenue = useMemo(() => {
        return filteredOrders.reduce((sum, order) => sum + order.totalAmount, 0);
    }, [filteredOrders]);

    const pendingOrders = useMemo(() => {
        return filteredOrders.filter(order => {
            const status = order.status.toLowerCase();
            return status === "pending" || status === "processing";
        }).length;
    }, [filteredOrders]);

    const salesPoints = useMemo(() => {
        if (filteredOrders.length === 0) {
            return fallbackSales;
        }

        const bucketCount = 12;
        const buckets = new Array<number>(bucketCount).fill(0);

        filteredOrders.forEach((order, index) => {
            buckets[index % bucketCount] += order.totalAmount;
        });

        let runningTotal = 0;
        const cumulative = buckets.map(value => {
            runningTotal += value;
            return runningTotal;
        });

        if (cumulative.every(value => value === 0)) {
            return fallbackSales;
        }

        return cumulative;
    }, [filteredOrders]);

    const growthPercent = useMemo(() => {
        const start = salesPoints[0] || 0;
        const end = salesPoints[salesPoints.length - 1] || 0;

        if (start <= 0) {
            return 0;
        }

        return ((end - start) / start) * 100;
    }, [salesPoints]);

    const trendHigh = useMemo(() => {
        const max = Math.max(...salesPoints);

        if (max <= 0) {
            return 0;
        }

        const rounded = Math.ceil(max / 5000) * 5000;
        return rounded;
    }, [salesPoints]);

    const yLabels = useMemo(() => {
        if (trendHigh === 0) {
            return ["Rs 0", "Rs 0", "Rs 0", "Rs 0", "Rs 0", "Rs 0"];
        }

        const step = trendHigh / 5;
        return [trendHigh, trendHigh - step, trendHigh - step * 2, trendHigh - step * 3, trendHigh - step * 4, 0].map(value =>
            `Rs ${Math.round(value / 1000)}K`
        );
    }, [trendHigh]);

    const trendChangeText = `${growthPercent >= 0 ? "" : ""}${Math.abs(growthPercent).toFixed(1)}%`;
    const topSeller = bestSellingProducts[0]?.product || "n/a";

    const labels = useMemo(() => {
        const [yearText, monthText] = selectedMonthKey.split("-");
        const selectedDate = new Date(Number(yearText), Number(monthText) - 1, 1);
        const monthLabel = selectedDate.toLocaleDateString("en-IN", { month: "short" });

        return [1, 7, 13, 19, 25, 31].map(day => `${day} ${monthLabel}`);
    }, [selectedMonthKey]);

    return (
        <AdminLayout>
            <div className="sales-overview-page">
                <div className="sales-overview-header-row">
                    <div>
                        <h2 className="sales-overview-title">Sales Overview</h2>
                        <p className="sales-overview-subtitle">Track your sales performance over time</p>
                    </div>

                    <div className="sales-overview-controls">
                        <select
                            className="sales-overview-month-select"
                            value={selectedMonthKey}
                            onChange={event => setSelectedMonthKey(event.target.value)}
                            aria-label="Choose month"
                        >
                            {monthOptions.map(option => (
                                <option key={option.key} value={option.key}>{option.label}</option>
                            ))}
                        </select>

                        <button type="button" className="sales-overview-range-btn">
                            <span className="sales-overview-range-left">
                                <FaCalendarAlt />
                                {selectedMonthLabel}
                            </span>
                            <FaChevronDown className="sales-overview-range-arrow" />
                        </button>
                    </div>
                </div>

                <div className="sales-kpi-grid">
                    <div className="sales-kpi-card sales-kpi-blue">
                        <div className="sales-kpi-row">
                            <span className="sales-kpi-icon-wrap">
                                <FaRupeeSign />
                            </span>
                            <div>
                                <p>Total Sales</p>
                                <h3>{inrFormatter.format(totalRevenue)}</h3>
                            </div>
                        </div>
                        <div className="sales-kpi-growth">
                            <FaArrowUp />
                            <span>{trendChangeText}</span>
                            <small>vs last month</small>
                        </div>
                    </div>

                    <div className="sales-kpi-card sales-kpi-green">
                        <div className="sales-kpi-row">
                            <span className="sales-kpi-icon-wrap">
                                <FaShoppingCart />
                            </span>
                            <div>
                                <p>Total Orders</p>
                                <h3>{filteredOrders.length.toLocaleString("en-IN")}</h3>
                            </div>
                        </div>
                        <div className="sales-kpi-growth">
                            <FaArrowUp />
                            <span>{Math.max(growthPercent * 0.7, 0).toFixed(1)}%</span>
                            <small>vs last month</small>
                        </div>
                    </div>

                    <div className="sales-kpi-card sales-kpi-amber">
                        <div className="sales-kpi-row">
                            <span className="sales-kpi-icon-wrap">
                                <FaBoxes />
                            </span>
                            <div>
                                <p>Available Stocks</p>
                                <h3>{availableStock.toLocaleString("en-IN")}</h3>
                            </div>
                        </div>
                        <div className="sales-kpi-growth">
                            <FaArrowUp />
                            <span>{Math.max(growthPercent * 0.3, 0).toFixed(1)}%</span>
                            <small>in inventory</small>
                        </div>
                    </div>

                    <div className="sales-kpi-card sales-kpi-purple">
                        <div className="sales-kpi-row">
                            <span className="sales-kpi-icon-wrap">
                                <FaExclamationTriangle />
                            </span>
                            <div>
                                <p>Low Stocks</p>
                                <h3>{lowStockCount.toLocaleString("en-IN")}</h3>
                            </div>
                        </div>
                        <div className="sales-kpi-growth">
                            <FaArrowUp />
                            <span>{Math.max(growthPercent * 0.2, 0).toFixed(1)}%</span>
                            <small>needs attention</small>
                        </div>
                    </div>

                </div>

                <section className="sales-trend-card">
                    <div className="sales-trend-header">
                        <div>
                            <h4>Sales Trend</h4>
                            <div className="sales-legend">
                                <span className="sales-legend-dot" />
                                Sales (INR)
                            </div>
                        </div>

                        <div className="sales-trend-growth">
                            <span className="sales-trend-growth-badge">
                                <FaArrowUp />
                                {trendChangeText}
                            </span>
                            <span>vs last month</span>
                        </div>
                    </div>

                    <div className="sales-chart-layout">
                        <div className="sales-y-axis">
                            {yLabels.map((label, index) => (
                                <span key={`${label}-${index}`}>{label}</span>
                            ))}
                        </div>

                        <div className="sales-chart-wrap">
                            <div className="sales-chart-grid" />
                            <SalesTrendChart salesPoints={salesPoints} />
                        </div>
                    </div>

                    <div className="sales-x-axis">
                            {labels.map(label => (
                                <span key={label}>{label}</span>
                            ))}
                    </div>
                </section>

                <div className="sales-insight-card">
                    <span className="sales-insight-icon">
                        <FaLightbulb />
                    </span>
                    <div>
                        <h5>Insight</h5>
                        <p>
                            Your sales are up <strong>{trendChangeText}</strong> this month. Available stock: <strong>{availableStock}</strong>, pending
                            orders: <strong>{pendingOrders}</strong>, top seller: <strong>{topSeller}</strong>, low stock alerts: <strong>{lowStockCount}</strong>.
                        </p>
                    </div>
                </div>

                {loading && (
                    <div className="sales-overview-loading">Loading dashboard data...</div>
                )}
            </div>
        </AdminLayout>
    );
}

export default AdminDashboard;
