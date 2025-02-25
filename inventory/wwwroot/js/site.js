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

        $(document).ready(function() {
            const maxVisibleNotifications = 3;
            let allNotifications = [];
            let showingAll = false;
        
            // Fetch notifications from the server
            function fetchNotifications() {
                $.get("/api/notifications", function(data) {
                    allNotifications = data; // Store all fetched notifications
                    displayNotifications();
                });
            }
        
            // Display notifications in the dropdown
            function displayNotifications() {
                const notificationsToShow = showingAll ? allNotifications : allNotifications.slice(0, maxVisibleNotifications);
                const notificationList = $('#notificationList');
                notificationList.empty(); // Clear current notifications
        
                notificationsToShow.forEach(notification => {
                    const notificationItem = $('<li class="list-group-item"></li>').text(notification.message);
                    notificationList.append(notificationItem);
                });
        
                $('#notificationCount').text(allNotifications.length);
                $('#viewAllNotifications').toggle(allNotifications.length > maxVisibleNotifications && !showingAll);
                $('#viewLessNotifications').toggle(showingAll);
            }
        
            // Event listeners for view more and view less
            $('#viewAllNotifications').click(function() {
                showingAll = true;
                displayNotifications();
            });
        
            $('#viewLessNotifications').click(function() {
                showingAll = false;
                displayNotifications();
            });
        
            // Initial fetch and setup periodic updates
            fetchNotifications();
            setInterval(fetchNotifications, 30000); // Refresh notifications every 30 seconds
        
            // Setup dropdown toggle behavior
            $('#notificationDropdown').on('click', function(e) {
                e.stopPropagation(); // Prevents the dropdown from closing immediately when clicked
                $(this).parent().toggleClass('show'); // Toggle the 'show' class that controls dropdown visibility
                $(this).next('.dropdown-menu').toggleClass('show'); // Toggle the visibility of the dropdown menu
            });
        
            // Close dropdown when clicking outside
            $(document).on('click', function(e) {
                if (!$(e.target).closest('.dropdown').length) {
                    $('.dropdown').removeClass('show');
                    $('.dropdown-menu').removeClass('show');
                }
            });
        });

        