$(document).ready(function() {
    // New Notification Button Toggle
    $("#newNotificationButton").on("click", function(e) {
        e.preventDefault();
        $("#newNotificationDropdown").toggleClass("show");
        $(this).attr("aria-expanded", $("#newNotificationDropdown").hasClass("show"));
    });

    // Mark All Read Functionality
    $("#markAllRead").on("click", function() {
        $(".notification-item").removeClass("unread");
        $(".notification-badge").hide();
        console.log("Marked all notifications as read.");
    });

    // Close Notification Dropdown on Outside Click
    $(document).on("click", function(e) {
        if (!$(e.target).closest(".notification-wrapper").length) {
            $("#newNotificationDropdown").removeClass("show");
            $("#newNotificationButton").attr("aria-expanded", "false");
        }
    });

    // Sidebar Toggle
    $(".sidebar-toggle").on("click", function() {
        $("#sidebar").toggleClass("hidden");
        $("#main-content").toggleClass("shrink");
    });

    // Close Sidebar on Outside Click
    $(document).on("click", function(e) {
        if (!$(e.target).closest(".sidebar, .sidebar-toggle").length && !$("#sidebar").hasClass("hidden")) {
            $("#sidebar").addClass("hidden");
            $("#main-content").removeClass("shrink");
        }
    });

    // Initial Animation for Stat Cards
    $(window).on("load", function() {
        $(".stat-card").each(function(index) {
            $(this).css("animation-delay", `${index * 0.1}s`);
        });
    });
});