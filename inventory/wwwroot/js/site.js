// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationsHub")
            .build();

        const maxVisibleNotifications = 3;
        let notifications = [];
        let isViewingMore = false;

        connection.on("ReceiveNotification", function (message) {
            const newItem = document.createElement("li");
            newItem.classList.add("list-group-item");
            newItem.innerHTML = `${message} <button class="close" onclick="removeNotification(this)">&times;</button>`;

            notifications.unshift(newItem); // Add to the start of the array
            updateNotificationDisplay();
        });

        function updateNotificationDisplay() {
            const notificationList = document.getElementById("notificationList");
            notificationList.innerHTML = ''; // Clear current notifications

            const visibleNotifications = isViewingMore ? notifications : notifications.slice(0, maxVisibleNotifications);
            visibleNotifications.forEach(item => {
                notificationList.appendChild(item);
            });

            document.getElementById("notificationCount").textContent = notifications.length;
            document.getElementById("notificationCount").className = `badge bg-${notifications.length > 0 ? 'primary' : 'secondary'}`;

            document.getElementById("viewMore").style.display = notifications.length > maxVisibleNotifications && !isViewingMore ? '' : 'none';
            document.getElementById("viewLess").style.display = isViewingMore ? '' : 'none';
        }

        function viewMore() {
            isViewingMore = true;
            updateNotificationDisplay();
        }

        function viewLess() {
            isViewingMore = false;
            updateNotificationDisplay();
        }

        function removeNotification(button) {
            const li = button.parentNode;
            notifications = notifications.filter(item => item !== li);
            updateNotificationDisplay();
        }

        connection.start().catch(function (err) {
            return console.error(err.toString());
        });
    
