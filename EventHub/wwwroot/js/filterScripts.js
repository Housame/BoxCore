/*The filter function is relevant when listing competitions, as on the make
reservation page, and the edit competition page*/
$(document).ready(function () {
    //The results-div is where the filtered competitions will be listed
    var results = $("#competitionDiv");
    var title = $(document).find("title").text();
    
    $("select").change(function () {
        /*Values are collected from all the filter-choice dropdowns..
        ..and sent asynchronously to back-end using AJAX */
        var IsInEditView;
        if (title === "Edit Competition - BoxCore") {
            IsInEditView = true;
        }
        else {
            IsInEditView = false;
        }
        var difficultyFilter = $("#filterDifficulty").val();
        var locationFilter = $("#filterLocation").val();
        
        $.ajax({
            url: "/Reservation/GetFilteredCompetitions",
            type: "GET",
            data: { difficulty: difficultyFilter, location: locationFilter, isInEditView: IsInEditView},
            success: function (filteredCompetitions) {
                results.html(filteredCompetitions);
            }
        });
    });
});