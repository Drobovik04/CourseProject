$.fn.dataTable.ext.type.order['custom-date-pre'] = function (data) {
    const parseDate = (dateString) => {
        if (!dateString) return 0;

        let parsedDate;
        if (dateString.includes('.')) {
            const [day, month, yearAndTime] = dateString.split('.');
            const [year, time] = yearAndTime.split(' ');
            parsedDate = new Date(`${year}-${month}-${day}T${time}`);
        } else {
            parsedDate = new Date(dateString);
        }

        return parsedDate.getTime() || 0;
    };

    return parseDate(data);
};
