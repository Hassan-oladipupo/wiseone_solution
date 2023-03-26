function leavedto() {
    this.financialYear = {
        startDate: moment(),
        endDate: moment(),
        opts: {
            showWeekNumbers: true,
            locale: {
                applyClass: 'btn-success',
                applyLabel: "Apply",
                fromLabel: "From",
                format: "DD/MM/YYYY",
                toLabel: "To",
                cancelLabel: 'Cancel',
            },
        },
        excludeMonths: [],
        Location: {},
        months: [],
        selectedMonth: {
            Month: '',
            Year: '',
            Days: '',
            Exclude: false,
            Label: ''
        }
    };
}