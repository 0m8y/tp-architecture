window.renderCalendar = function (reservations) {
    if (!reservations || !Array.isArray(reservations)) {
        console.error("Paramètre 'reservations' invalide :", reservations);
        return;
    }

    const calendarEl = document.getElementById('calendar');
    if (!calendarEl) return;
    function getDeterministicColor(index) {
        const colors = ['#FF6B6B', '#6BCB77', '#4D96FF', '#FFD93D', '#A66DD4', '#FF9F1C', '#70D6FF', '#FF70A6'];
        return colors[index % colors.length];
    }

    const events = reservations.map((r, index) => {
        console.log(`Reservation ID: ${r.id}`);
        console.log(`Start: ${r.startDate}, End: ${r.endDate}`);

        return {
            id: r.roomNumber,
            title: `${r.roomIds.length} chambre(s) - ${r.status}`,
            start: r.startDate,
            end: r.endDate,
            color: getDeterministicColor(index)
        };
    });

    const calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        height: "auto",
        events: events,
        //eventClick: function (info) {
        //    const confirmCancel = confirm("Souhaitez-vous annuler cette réservation ?");
        //    if (confirmCancel) {
        //        fetch(`/api/v1/bookings/reservations/${info.event.id}/cancel`, {
        //            method: 'POST',
        //            headers: { 'Authorization': `Bearer ${localStorage.getItem('authToken')}` }
        //        }).then(() => {
        //            alert("Réservation annulée !");
        //            info.event.remove();
        //        });
        //    }
        //}
    });

    calendar.render();
}
