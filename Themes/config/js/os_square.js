$(document).ready(function () {

    $('#OS_Square_cmdSave').unbind("click");
    $('#OS_Square_cmdSave').click(function () {
        $('.processing').show();
        $('.actionbuttonwrapper').hide();
        // lower case cmd must match ajax provider ref.
        nbxget('os_square_savesettings', '.OS_Squaredata', '.OS_Squarereturnmsg');
    });

    $('.selectlang').unbind("click");
    $(".selectlang").click(function () {
        $('.editlanguage').hide();
        $('.actionbuttonwrapper').hide();
        $('.processing').show();
        $("#nextlang").val($(this).attr("editlang"));
        // lower case cmd must match ajax provider ref.
        nbxget('os_square_selectlang', '.OS_Squaredata', '.OS_Squaredata');
    });

    $(document).on("nbxgetcompleted", OS_Square_nbxgetCompleted); // assign a completed event for the ajax calls

    // function to do actions after an ajax call has been made.
    function OS_Square_nbxgetCompleted(e) {

        $('.processing').hide();
        $('.actionbuttonwrapper').show();
        $('.editlanguage').show();

        if (e.cmd == 'os_square_selectlang') {
                        
        }

    };

});

