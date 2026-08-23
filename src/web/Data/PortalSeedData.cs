namespace NordicBike.Portal;

public static class PortalSeedData
{
    public static void Seed(this PortalStore store)
    {
        store.Orders.AddRange([
            SeedOrder(store, "NB-ORD-20250310-001", "NB-CUST-10041", "Delivered", "Malmo", "aurora-x3", -120, "NBX-77124"),
            SeedOrder(store, "NB-ORD-20260722-002", "NB-CUST-10041", "Shipped", "Malmo", "led-light-set", -4, "NBX-88102"),
            SeedOrder(store, "NB-ORD-20260801-003", "NB-CUST-10042", "Processing", "Stockholm", "fjord-cargo", -2),
            SeedOrder(store, "NB-ORD-20260816-004", "NB-CUST-10043", "Confirmed", "Gothenburg", "vinter-pro", -1),
            SeedOrder(store, "NB-ORD-20260701-005", "NB-CUST-10043", "Delivered", "Gothenburg", "pannier-bag", -49, "NBX-77293"),
            SeedOrder(store, "NB-ORD-20260502-006", "NB-CUST-10044", "Returned", "Umea", "phone-mount", -100),
            SeedOrder(store, "NB-ORD-20260810-007", "NB-CUST-10042", "Cancelled", "Stockholm", "frame-lock", -9),
            SeedOrder(store, "NB-ORD-20260620-008", "NB-CUST-10044", "Return requested", "Umea", "powerpack-720", -60)
        ]);

        store.Bikes.AddRange([
            new Bike { Id = "BIKE-001", CustomerId = "NB-CUST-10041", ProductId = "aurora-x3", ProductName = "Aurora X3", Serial = "AX3-25A-00417", Purchased = new(2025, 3, 10), Registration = "Verified", ServiceStatus = "Ready to ride" },
            new Bike { Id = "BIKE-002", CustomerId = "NB-CUST-10042", ProductId = "fjord-cargo", ProductName = "Fjord Cargo", Serial = "FJC-25B-00113", Purchased = new(2025, 12, 4), Registration = "Verified", ServiceStatus = "Ready to ride" },
            new Bike { Id = "BIKE-003", CustomerId = "NB-CUST-10043", ProductId = "vinter-pro", ProductName = "Vinter Pro", Serial = "VTP-24A-00201", Purchased = new(2024, 7, 15), Registration = "Verified", ServiceStatus = "Inspection booked" }
        ]);

        store.SupportIssues.AddRange([
            new("ISSUE-001", "Checkout redirect points at a nonexistent order", "Easy", "Checkout", "Change only the client redirect formatting after a successful checkout.", "customer report, browser URL, API response, and two correlated request logs", "compare the created canonical ID with the UI navigation value", "the wrong redirect reproduces reliably and the fix restores /orders/{id}", ["Open Shop.", "Add any product to the cart.", "Open Cart.", "Continue to checkout.", "Complete the form, check the simulation box, and place the order."], "Planned"),
            new("ISSUE-002", "Cart total does not refresh after a quantity update", "Easy", "Cart", "Preserve the API mutation but render the summary from a stale client snapshot.", "support screenshot, cart audit event, and API response with the correct quantity", "isolate UI state from API mutation behavior", "quantity updates persist and the summary refreshes from current data", ["Open Shop.", "Add a product to the cart.", "Open Cart.", "Change a line quantity.", "Click Update and verify the total stays stale until refresh."], "Planned"),
            new("ISSUE-003", "Shipped order remains Processing for a customer", "Moderate", "Orders", "Omit Shipped from the customer-status projection but retain it for operations.", "customer report, business screenshot, status-change audit, and partial API payload", "trace one state transition through entity, DTO, and view mapping", "customer view matches internal Shipped state after the fix", ["Switch to Support agent or Support lead.", "Open Orders.", "Open an order.", "Advance fulfilment until the status becomes Shipped.", "Switch back to Anna Karlsson and reopen the same order."], "Planned"),
            new("ISSUE-004", "Support agent cannot create a Malmo service request", "Moderate", "Support", "Add whitespace or normalization mismatch to the selected service-center value.", "agent report, 400 problem-details response, submitted payload, unrelated old warnings", "find the controlled-vocabulary validation seam; do not confuse it with warranty eligibility", "valid Malmo requests succeed after the fix", ["Switch to Support agent.", "Open a support case.", "Open the case page.", "In Case controls, select Malmo as the service center.", "Click Create service request."], "Planned"),
            new("ISSUE-005", "Vinter Pro owner is recommended an incompatible battery", "Moderate", "Catalog", "Reverse one UI compatibility lookup while catalog API data stays correct.", "product screenshot, catalog API response, customer conversation, and a distracting warranty note", "diagnose UI/API inconsistency rather than changing policy data", "UI and API recommendations match after the fix", ["Open Shop.", "Open Vinter Pro.", "Read the Recommended battery text.", "Compare it with the expected battery for the model."], "Planned"),
            new("ISSUE-006", "Support agent clears escalation with no audit trail", "Hard", "Support workspace", "Permit one PATCH branch to clear escalation without checking the role or recording an audit event.", "case timeline, diagnostics extract, partial console logs, and compensation discussion noise", "compare all authorization paths and use missing observability evidence carefully", "only support leads can clear escalation and each clearance is audited", ["Switch to Support agent.", "Open Support.", "Open an escalated case.", "Clear Escalated.", "Save the case and check Diagnostics."], "Planned"),
            new("ISSUE-007", "Customer sees internal note after switching roles", "Hard", "Support workspace", "Cache an internal case projection and reuse it after the role cookie changes.", "exact reproduction sequence, navigation timing, sanitized network capture, unrelated correlation IDs", "reason about state lifetime, identity boundaries, and projection filtering", "customer view never leaks internal notes after role changes", ["Switch to Support agent.", "Open Support.", "Open a customer case.", "Add an internal note.", "Switch to Anna Karlsson and reopen the same case."], "Planned"),
            new("ISSUE-008", "Slow connection creates duplicate checkout orders", "Hard", "Checkout", "Remove client submit protection and process repeated POSTs without idempotency handling.", "similar orders, two correlation IDs, timing data, and unrelated payment terminology", "distinguish retry from refresh, prove source, then propose idempotency without blocking later valid purchases", "duplicate checkout orders no longer occur after the fix", ["Add products to the cart.", "Open Checkout.", "Fill in the form.", "Click Place simulated order twice quickly or double-click it."], "Planned")
        ]);

        AddCase(store, "NB-CASE-00201", "NB-CUST-10041", "Intermittent charging on Aurora X3", "Bike or battery fault", "High", "Waiting for service center", "Sofia Nilsson", false, "NB-ORD-20250310-001", "BIKE-001", -5);
        AddCase(store, "NB-CASE-00202", "NB-CUST-10042", "Tracking reference has not updated", "Order and delivery", "Normal", "In progress", "Oskar Bergman", false, "NB-ORD-20260801-003", null, -2);
        AddCase(store, "NB-CASE-00203", "NB-CUST-10043", "Requesting a compensation payment", "Warranty and repair", "High", "In progress", "Sofia Nilsson", true, "NB-ORD-20260701-005", "BIKE-003", -6);
        AddCase(store, "NB-CASE-00204", "NB-CUST-10044", "Return request for PowerPack 720", "Return request", "Normal", "New", null, false, "NB-ORD-20260620-008", null, -1);
        AddCase(store, "NB-CASE-00205", "NB-CUST-10041", "Can I add a phone mount?", "Product question", "Low", "Resolved", "Oskar Bergman", false, null, "BIKE-001", -12);
        AddCase(store, "NB-CASE-00206", "NB-CUST-10043", "Winter range question", "Product question", "Normal", "Waiting for customer", "Sofia Nilsson", false, "NB-ORD-20260816-004", null, -3);
        foreach (var order in store.Orders) store.Audit("seed", "System", "Seeded order", order.Id, order.Status);
    }

    private static Order SeedOrder(PortalStore store, string id, string customer, string status, string city, string product, int daysAgo, string? tracking = null)
    {
        var item = store.Products.Single(productItem => productItem.Id == product);
        var at = DemoClock.Now.AddDays(daysAgo);
        var order = new Order
        {
            Id = id,
            CustomerId = customer,
            Lines = [new OrderLine(item.Id, item.Name, 1, item.Price, null)],
            Status = status,
            City = city,
            CreatedAt = at,
            UpdatedAt = at.AddHours(3),
            Tracking = tracking
        };
        order.Timeline.Add(new TimelineEvent(at, "Order confirmed", "Your simulated order was accepted."));
        order.Timeline.Add(new TimelineEvent(at.AddHours(3), status, $"Order is now {status.ToLowerInvariant()}."));
        return order;
    }

    private static void AddCase(PortalStore store, string id, string customer, string subject, string topic, string priority, string status, string? assignee, bool escalated, string? orderId, string? bikeId, int daysAgo)
    {
        var at = DemoClock.Now.AddDays(daysAgo);
        var supportCase = new SupportCase
        {
            Id = id,
            CustomerId = customer,
            Subject = subject,
            Description = "Seeded demo case for the NordicBike portal.",
            Topic = topic,
            Priority = priority,
            Status = status,
            Assignee = assignee,
            Escalated = escalated,
            OrderId = orderId,
            BikeId = bikeId,
            CreatedAt = at,
            UpdatedAt = at.AddHours(2)
        };
        supportCase.Messages.Add(new CaseMessage(store.Customers.Single(item => item.Id == customer).Name, "Customer", "Customer", "I need help with this item.", at));
        if (escalated) supportCase.Messages.Add(new CaseMessage("Sofia Nilsson", "Support lead", "Internal", "Escalated because the cash compensation request exceeds support authority.", at.AddHours(1)));
        supportCase.Timeline.Add(new TimelineEvent(at, "Case created", topic));
        supportCase.Timeline.Add(new TimelineEvent(at.AddHours(2), status, "Seeded workflow state."));
        store.Cases.Add(supportCase);
    }
}
