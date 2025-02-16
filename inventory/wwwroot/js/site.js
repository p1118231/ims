// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function() {
    // Function to fetch and display notifications
    function loadNotifications() {
        // Simulate fetching data
        const notifications = ['New order placed', 'Item 123 low in stock', 'New supplier added'];
        const list = document.getElementById('notificationList');
        list.innerHTML = '';
        notifications.forEach(notif => {
            const li = document.createElement('li');
            li.className = 'list-group-item';
            li.textContent = notif;
            list.appendChild(li);
        });
    }

    // Function to update inventory health
    function updateInventory() {
        document.getElementById('currentStock').textContent = '500 items';
        document.getElementById('lowStockCount').textContent = '15 items';
        document.getElementById('overstockCount').textContent = '8 items';
    }

    loadNotifications();
    updateInventory();
});
