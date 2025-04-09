const notyf = new Notyf({
    duration: 10000, // Default duration for notifications
    position: {
        x: 'right',
        y: 'top',
    },
    types: [
        {
            type: 'error',
            background: 'indianred',
            duration: 10000, // Duration for error notifications
            dismissible: true
        },
        {
            type: 'success',
            dismissible: true,
            duration: 10000, // Duration for success notifications
        },
        {
            type: 'warning',
            background: 'orange',
            icon: '<i class="material-icons">warning</i>',
            dismissible: true
        },
        {
            type: 'info',
            icon: '<i class="material-icons">info</i>',
            background: '#1e90ff',
            dismissible: true,
            duration: 10000 // Duration for info notifications
        }
    ]
});