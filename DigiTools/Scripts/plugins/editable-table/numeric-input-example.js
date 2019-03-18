/* global $ */
/* this is an example for validation and change events */
$.fn.numericValidation = function () {
    'use strict';   
    var element = $(this),
        footer = element.find('tfoot tr'),
        dataRows = element.find('tbody tr');
		
    element.find('td').on('validate', function (evt, value) {       
		var cell = $(this),
			column = cell.index();
        if (column === 1 || column === 2) {
			return !!value && value.trim().length > 0;
		} else {
			return !isNaN(parseFloat(value)) && isFinite(value);
		}
	});
	return this;
};
