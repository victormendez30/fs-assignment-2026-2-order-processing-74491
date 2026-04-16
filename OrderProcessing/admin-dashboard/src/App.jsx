import { useEffect, useState } from "react";

function App() {
  const [orders, setOrders] = useState([]);
  const [filter, setFilter] = useState("All");
  const [selectedOrder, setSelectedOrder] = useState(null);

  useEffect(() => {
    const loadOrders = async () => {
      try {
        const res = await fetch("/api/orders");
        const data = await res.json();
        setOrders(data);
      } catch (err) {
        console.error(err);
      }
    };

    loadOrders();

    const interval = setInterval(loadOrders, 5000);

    return () => clearInterval(interval);
  }, []);

  const filteredOrders =
    filter === "All"
      ? orders
      : orders.filter((o) => o.status === filter);

  const summary = {
    total: orders.length,
    completed: orders.filter((o) => o.status === "Completed").length,
    failed: orders.filter((o) => o.status === "Failed").length,
    processing: orders.filter(
      (o) => o.status !== "Completed" && o.status !== "Failed"
    ).length
  };

  return (
    <div style={{ padding: 20 }}>
      <h1>Admin Dashboard</h1>

      {/* SUMMARY */}
      <div style={{ display: "flex", gap: 20, marginBottom: 20 }}>
        <div style={{ border: "1px solid #ccc", padding: 12 }}>
          <strong>Total Orders</strong>
          <div>{summary.total}</div>
        </div>
        <div style={{ border: "1px solid #ccc", padding: 12 }}>
          <strong>Completed</strong>
          <div>{summary.completed}</div>
        </div>
        <div style={{ border: "1px solid #ccc", padding: 12 }}>
          <strong>Failed</strong>
          <div>{summary.failed}</div>
        </div>
        <div style={{ border: "1px solid #ccc", padding: 12 }}>
          <strong>Processing</strong>
          <div>{summary.processing}</div>
        </div>
      </div>

      {/* FILTER */}
      <div style={{ marginBottom: 20 }}>
        <label>Status: </label>
        <select value={filter} onChange={(e) => setFilter(e.target.value)}>
          <option>All</option>
          <option>Submitted</option>
          <option>InventoryPending</option>
          <option>InventoryConfirmed</option>
          <option>PaymentPending</option>
          <option>PaymentApproved</option>
          <option>ShippingPending</option>
          <option>ShippingCreated</option>
          <option>Completed</option>
          <option>Failed</option>
        </select>
      </div>

      {/* TABLE */}
      <table border="1" cellPadding="10" width="100%">
        <thead>
          <tr>
            <th>Order Id</th>
            <th>Customer</th>
            <th>Status</th>
            <th>Total</th>
          </tr>
        </thead>
        <tbody>
          {filteredOrders.map((o) => (
            <tr
              key={o.id}
              onClick={() => setSelectedOrder(o)}
              style={{
                cursor: "pointer",
                backgroundColor:
                  o.status === "Failed"
                    ? "#ffd6d6"
                    : o.id === selectedOrder?.id
                    ? "#e6f7ff"
                    : "white"
              }}
            >
              <td>{o.id}</td>
              <td>{o.customerName}</td>
              <td>{o.status}</td>
              <td>{o.totalAmount}</td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* DETAILS */}
      {selectedOrder && (
        <div style={{ marginTop: 20, border: "1px solid #ccc", padding: 16 }}>
          <h2>Order Details</h2>
          <p><strong>Order Id:</strong> {selectedOrder.id}</p>
          <p><strong>Customer:</strong> {selectedOrder.customerName}</p>
          <p><strong>Status:</strong> {selectedOrder.status}</p>
          <p><strong>Total:</strong> {selectedOrder.totalAmount}</p>
          <p><strong>Created At:</strong> {selectedOrder.createdAt}</p>
          <p><strong>Completed At:</strong> {selectedOrder.completedAt || "-"}</p>
          <p><strong>Failure Reason:</strong> {selectedOrder.failureReason || "-"}</p>
        </div>
      )}
    </div>
  );
}

export default App;