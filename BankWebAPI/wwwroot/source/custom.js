
openPage = function (verb, url, data, target) {
    var form = document.createElement("form");
    form.action = url;
    form.method = verb;
    form.target = target || "_self";
    if (data) {
        for (var key in data) {
            var input = document.createElement("textarea");
            input.name = key;
            input.value = typeof data[key] === "object" ? JSON.stringify(data[key]) : data[key];
            form.appendChild(input);
        }
    }
    form.style.display = 'none';
    document.body.appendChild(form);
    form.submit();
};

var _AutoUnblockUI = true;
var _weburl = 'https://localhost:44301/';


DataTableModel = function (d) {
    return {
        GridDraw: d.draw,
        GridStart: d.start,
        GridLength: d.length,
        GridColumns: d.columns,
        GridSearch: d.search,
        GridOrder: d.order,
    };
};


var MenuTools = function () {
    var areaIcon = function () {
        var url = $(location).attr("href");
        var urlData = url.replace(_weburl, '').split('/');
        var area = urlData[0];
        $('.box-icon-area').each(function (i, obj) {
            var $obj = $(this);
            if ($obj.attr('data-area').toUpperCase() == area.toUpperCase()) {
                $obj.css({
                    color: '#ff005e'
                });
            } else {
                // $obj.closest('.box-home').hide();
            }
        });
    }
    var activeSubMenu = function () {
        var url = $(location).attr("href");
        var urlData = url.replace(_weburl, '').split('/');
        var area = urlData[0];
        var controller = urlData[1];
        var action = urlData[2];

        if (area.toUpperCase() == 'SALE') {
            $('.textProject').html('Front Office App. (Sales)');
        } else if (area.toUpperCase() == 'HLTC') {
            $('.textProject').html('HLTC Portal Applications');
            $('.leftBlock').css({ width: '90px' });
            $('.closeMenu').hide();
        } else if (area.toUpperCase() == 'TRACKING') {
            $('.textProject').html('Customer Status Tracking');
        } else if (area.toUpperCase() == 'CUSTOMERSERVICE') {
            $('.textProject').html('Customer Service Application');
        } else if (area.toUpperCase() == 'GENERALLEDGER') {
            $('.textProject').html('Accounting Application');
        } else if (area.toUpperCase() == 'MARKETINGCRM') {
            $('.textProject').html('Marketing & CRM');
        } else {
            $('.textProject').html('HLTC Portal Applications');
        }

        $('.liSubMenu').each(function (i, item) {
            var $obj = $(this);
            var actionCount = 0;
            var itemArea = $obj.attr('data-area');
            var itemController = $obj.attr('data-controller');
            var itemAction = $obj.attr('data-actionUrl');
            var actionSplit = itemAction.split(',');
            if (controller !== undefined && action !== undefined) {
                $.each(actionSplit, function (a, act) {
                    if (act.toUpperCase() == action.toUpperCase()) {
                        actionCount++;
                    }
                });

                if (itemArea.toUpperCase() == area.toUpperCase() && itemController.toUpperCase() == controller.toUpperCase() && actionCount >= 1) {
                    $obj.addClass('submenuActive')
                        .closest('ul').css({
                            'display': 'block'
                        })
                        .closest('li').addClass('open');

                    console.log('Area = ' + itemArea + ', Controller = ' + itemController + ', Action = ' + itemAction);
                }
            }
        });

    }

    var genMenuLink = function () {
        $('.liSubMenu').each(function (i, item) {
            var $obj = $(this);
            var itemArea = $obj.attr('data-area');
            var itemController = $obj.attr('data-controller');
            var itemAction = $obj.attr('data-action');
            $obj.attr('href', _weburl + itemArea + '/' + itemController + '/' + itemAction);
        });

        $('.navbar-brand').attr('href', _weburl + 'HLTC/Home/Index');
    }

    var mobileScreen = function () {
        if ($(window).width() < 700) {
            $('#divRightBlock').removeClass('row').removeClass('rightBlock');
        }
    }

    return {
        setActive: function () {
            genMenuLink();
            activeSubMenu();
            mobileScreen();
        },
        areaIcon: function () {
            areaIcon();
        }
    }
}();

var AlertTools = function () {
    return {
        Error: function (msg) {
            if (msg == "" || msg == null) {
                msg = 'ขออภัยไม่สามารถทำรายการได้';
            }
            $.toast({
                heading: 'Error !',
                text: msg,
                position: 'top-right',
                bgColor: '#FF1356',
                textColor: 'white',
                showHideTransition: 'fade',
                icon: 'error',
                hideAfter: 3000
            });
        },
        Success: function (msg, mode) {
            if (msg == "" || msg == null) {
                msg = 'ขออภัยไม่สามารถทำรายการได้';
            }
            if (mode == 'save') {
                msg = 'บันทึกข้อมูลเรียบร้อยแล้ว';
            }
            $.toast({
                heading: 'Successfully !',
                text: msg,
                position: 'top-right',
                bgColor: '#007bff',
                textColor: 'white',
                showHideTransition: 'fade',
                icon: 'success',
                hideAfter: 3000
            });
        }
    }
}();

var AjaxTools = function () {
    var PostEvent = function (url, req, successCall, errorCall, isLoading) {
        if (isLoading) {
            LoadingTools.blockUI();
            if (errorCall == null) {
                errorCall = function () {
                    LoadingTools.unblockUI();
                }
            }
        }
        return $.ajax({
            url: url,
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(req),
            success: successCall,
            error: errorCall
        }).done(function () {
            if (isLoading) {
                LoadingTools.unblockUI();
            }
        });
    }

    var GetEvent = function (url, req, successCall, errorCall, isLoading) {
        if (isLoading) {
            LoadingTools.blockUI();
            if (errorCall == null) {
                errorCall = function () {
                    LoadingTools.unblockUI();
                }
            }
        }
        return $.ajax({
            url: url,
            type: "GET",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(req),
            success: successCall,
            error: errorCall
        }).done(function () {
            if (isLoading) {
                LoadingTools.unblockUI();
            }
        });
    }
    return {
        Post: function (url, req, success, error, isLoading) {
            return PostEvent(url, req, success, error, isLoading);
        },
        Get: function (url, req, success, error, isLoading) {
            return GetEvent(url, req, success, error, isLoading);
        }
    }
}();

var LoadingTools = function () {
    var blockUI = function () {
        $.blockUI({
            message: '<img src="' + _weburl + 'img/ajax-loader.gif" height="80px" width="80px" />',
            css: {
                backgroundColor: 'none',
                border: '0px',
                // backgroundImage: 'url("/images/ajax-loader.gif")',
                backgroundRepeat: 'no-repeat',
                backgroundPosition: '50% 0',
            }
        });
    }

    var unblockAuto = function () {
        if (_AutoUnblockUI) {
            $.unblockUI();
        }
    }

    var unblockUI = function () {
        $.unblockUI();
    }

    var disable = function () {
        _AutoUnblockUI = false;
    }

    var isAuto = function () {
        return _AutoUnblockUI;
    }

    return {
        blockUI: function () {
            blockUI();
        },
        unblockUI: function () {
            unblockUI();
        },
        disable: function () {
            disable();
        },
        isAuto: function () {
            return isAuto();
        },
        unblockAuto: function () {
            unblockAuto();
        }
    }
}();

var DateTimeTools = function () {
    return {
        datePicker: function (val) {
            if (val != null && val != '') {
                return moment(val, "DD/MM/YYYY").tz("Asia/Bangkok").format();
            } else {
                return '';
            }
        },
        dateTimePicker: function (val) {
            if (val != null && val != '') {
                return moment(val, "DD/MM/YYYY HH:mm").tz("Asia/Bangkok").format();
            } else {
                return '';
            }
        },
        now: function () {
            return moment.tz("Asia/Bangkok").format();
        },
        nowText: function () {
            return moment.tz("Asia/Bangkok").format('DD MMM YYYY HH:mm');
        },
        nowOb: function () {
            return new Date(moment.tz("Asia/Bangkok").format("YYYY-MM-DD HH:mm:ss"));
        },
        dateFormat: function () {
            return "dd/mm/yyyy";
        },
        textToApiDate: function (text, format) {
            if (text == null)
                return null;

            if (format == 'dd-MM-YYYY') {
                text = text.split('T');
                text = text[0].split('-');
                return text[2] + '-' + text[1] + '-' + text[0];
            }
            return null;
        },
        textToDate: function (text) {
            if (text == null)
                return null;
            var getDate = text.split('T');
            var dateSplit = getDate[0].split('-');
            return new Date(parseInt(dateSplit[0]), parseInt(dateSplit[1]) - 1, parseInt(dateSplit[2]));
        },
        textToDateTime: function (text) {
            if (text == null)
                return null;
            var getDate = text.split('T');
            var dateSplit = getDate[0].split('-');
            return new Date(parseInt(dateSplit[0]), parseInt(dateSplit[1]) - 1, parseInt(dateSplit[2]));
        },
        dateToFormat: function (text) {
            if (text == null)
                return '';
            var getDate = text.split('T');
            var dateSplit = getDate[0].split('-');
            return dateSplit[2] + "/" + dateSplit[1] + "/" + dateSplit[0];
        },
        dateTimeToFormat: function (text) {
            if (text == null)
                return '';
            var getDate = text.split('T');
            var dateSplit = getDate[0].split('-');
            if (getDate[1] === undefined)
                return '';
            return dateSplit[2] + "/" + dateSplit[1] + "/" + dateSplit[0] + " " + getDate[1].substr(0, 5);
        },
        setDateRange: function (fromDate, toDate, DateFormat) {
            $(fromDate).datepicker("destroy");
            $(toDate).datepicker("destroy");

            DateFormat = DateFormat || "dd/mm/yyyy";
            $(fromDate).datepicker({
                format: DateFormat,
                autoclose: true,
                clearBtn: true,
                todayHighlight: true,
            }).on('changeDate', function () {
                $(toDate).datepicker('setStartDate', null);
                if ($(this).val() != "") {
                    $(toDate).datepicker('setStartDate', new Date(moment($(this).val(), "DD/MM/YYYY").toDate()));
                }
            });

            $(toDate).datepicker({
                format: DateFormat,
                autoclose: true,
                clearBtn: true,
                todayHighlight: true,
            }).on('changeDate', function () {
                $(fromDate).datepicker('setEndDate', null);
                if ($(this).val() != "") {
                    $(fromDate).datepicker('setEndDate', new Date(moment($(this).val(), "DD/MM/YYYY").toDate()));
                }
            });
        }
    }
}();



var NumberTools = function () {
    return {
        stringFloat: function (text) {
            if (text !== undefined) {
                var count = text.indexOf(',');
                if (count > 0) {
                    return parseFloat(text.replace(/,/g, ''));
                }
            }
            return parseFloat(text);
        },
        textBox: function ($obj, digit) {
            $obj.unbind('keypress').bind("keypress", function (event) {
                if ((event.charCode > 47 && event.charCode < 58) || event.charCode === 0 || event.charCode === 46)
                    return true;
                else
                    return false;
            });

            if (digit == 2) {
                $obj.on('blur', function () {
                    var number = numeral($obj.val().trim()).format('0,0.00');
                    $obj.val(number);
                }).on('focus', function () {
                    var number = numeral($obj.val().trim()).format('0.00');
                    if (number == '0.00')
                        $obj.val('');
                    else
                        $obj.val(number);
                });
            }
        }
    }
}();

var SelectOptionTools = function () {
    var binding = function (req) {
        AjaxTools.Post(req.Url, req.Req, function (res) {
            var appHtml = '<select id="' + req.Id + '" name="' + req.Id + '">';
            if (req.Element !== undefined) {
                if (req.Class !== undefined) {
                    appHtml = '<select class="' + req.Class + '" name="' + req.Class + '" ' + req.Element + '>';
                } else {
                    appHtml = '<select id="' + req.Id + '" name="' + req.Id + '" ' + req.Element + '>';
                }
            }
            if (req.Class !== undefined) {
                appHtml = '<select class="' + req.Class + '" name="' + req.Class + '">';
            }
            if (req.IsRequire) {
                appHtml += '<option value="">- กรุณาเลือก -</value>';
            }

            $.each(res, function (i, obj) {
                if (req.Data == obj.id) {
                    appHtml += '<option value="' + obj.id + '" selected>' + obj.text + '</value>';
                } else {
                    appHtml += '<option value="' + obj.id + '">' + obj.text + '</value>';
                }
            });
            appHtml += '</select>';
            $(req.Box).html(appHtml);
            var slcId = "#" + req.Id;
            if (req.Class !== undefined) {
                slcId = "." + req.Class;
            }
            if (req.Position !== undefined) {
                req.Position();
            } else {
                $(slcId).select2();
            }
        }, null, true);
    }

    var bindingData = function (req) {
        var appHtml = '<select id="' + req.Id + '" name="' + req.Id + '">';
        if (req.Element !== undefined) {
            if (req.Class !== undefined) {
                appHtml = '<select class="' + req.Class + '" name="' + req.Class + '" ' + req.Element + '>';
            } else {
                appHtml = '<select id="' + req.Id + '" name="' + req.Id + '" ' + req.Element + '>';
            }
        }
        if (req.Class !== undefined) {
            appHtml = '<select class="' + req.Class + '" name="' + req.Class + '">';
        }
        if (req.IsRequire) {
            appHtml += '<option value="">- กรุณาเลือก -</value>';
        }

        $.each(req.Res, function (i, obj) {
            if (req.Data == obj.id) {
                appHtml += '<option value="' + obj.id + '" selected>' + obj.text + '</value>';
            } else {
                appHtml += '<option value="' + obj.id + '">' + obj.text + '</value>';
            }
        });
        appHtml += '</select>';
        $(req.Box).html(appHtml);
        var slcId = "#" + req.Id;
        if (req.Class !== undefined) {
            slcId = "." + req.Class;
        }
        if (req.Position !== undefined) {
            req.Position();
        } else {
            $(slcId).select2();
        }

    }
    return {
        binding: function (req) {
            binding(req);
        },
        bindingData: function (req) {
            bindingData(req);
        }
    }
}();

var StringTools = function () {
    var getUrlParam = function getUrlParameter(sParam) {
        var vars = {};
        var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
            vars[key] = value;
        });
        return vars[sParam];
    };
    return {
        getUrlParam: function (sParam) {
            return getUrlParam(sParam);
        }
    }
}();

var ValidateTools = function () {
    var MessagePosition = function (error, element) {
        if (element.hasClass('select2-hidden-accessible')) {
            error.insertAfter(element.parent().find('.select2-container'));
        } else if (element.hasClass('select2') && element.next('.select2-container').length) {
            error.insertAfter(element.next('.select2-container'));
        } else if (element.parent('.input-group').length) {
            error.insertAfter(element.parent());
        } else if (element.prop('type') === 'radio' && element.parent('.radio-inline').length) {
            error.insertAfter(element.parent().parent());
        } else if (element.prop('type') === 'checkbox' || element.prop('type') === 'radio') {
            error.appendTo(element.parent().parent());
        } else {
            error.insertAfter(element);
        }
    }
    return {
        MessagePosition: function (error, element) {
            MessagePosition(error, element);
        }
    }
}();

var TableTools = function () {

    var oldExportAction = function (self, e, dt, button, config) {
        if (button[0].className.indexOf('buttons-excel') >= 0) {
            if ($.fn.dataTable.ext.buttons.excelHtml5.available(dt, config)) {
                $.fn.dataTable.ext.buttons.excelHtml5.action.call(self, e, dt, button, config);
            }
            else {
                $.fn.dataTable.ext.buttons.excelFlash.action.call(self, e, dt, button, config);
            }
        } else if (button[0].className.indexOf('buttons-print') >= 0) {
            $.fn.dataTable.ext.buttons.print.action(e, dt, button, config);
        }
    };

    var ExportExcel = function (e, dt, button, config) {
        var self = this;
        var oldStart = dt.settings()[0]._iDisplayStart;

        dt.one('preXhr', function (e, s, data) {
            // Just this once, load all data from the server...
            data.GridStart = 0;
            data.GridLength = 999999999;

            dt.one('preDraw', function (e, settings) {
                // Call the original action function 
                oldExportAction(self, e, dt, button, config);

                dt.one('preXhr', function (e, s, data) {
                    // DataTables thinks the first item displayed is index 0, but we're not drawing that.
                    // Set the property to what it was before exporting.
                    settings._iDisplayStart = oldStart;
                    data.start = oldStart;
                });

                // Reload the grid with the original page. Otherwise, API functions like table.cell(this) don't work properly.
                // setTimeout(dt.ajax.reload, 0);

                // Prevent rendering of the full data to the DOM
                return false;
            });
        });

        // Requery the server with the new one-time export settings
        dt.ajax.reload();
    }
    return {
        ExportExcel: function (e, dt, button, config) {
            ExportExcel(e, dt, button, config)
        }
    }
}();

var LocalStorage = {
    Get: (name) => {
        let ret = false;
        if (name) {
            let data = window.localStorage.getItem(name);
            if (data != undefined && data != null && data != "" && data != "undefined") {
                ret = JSON.parse(data);
            }
        }
        return ret;
    },
    Set: (name, data) => {
        if (name) {
            window.localStorage.setItem(name, JSON.stringify(data));
        }
    }
}

