var vehiclesPanel = function (vehiclesJson) {
    return {
        generateTable: function () {
            var table = $("<table id='tbVehiclelist'></table>");

            for (var i = 0; i < vehiclesJson.length; i++) {
                var currentVehicle = vehiclesJson[i];

                var tr1 = $("<tr></tr>");
                var tr1Content = $('<td rowspan="5" class="vehicleImg"><img id="imgVehicle" alt="' + currentVehicle.name + '" src="' + currentVehicle.path + '" /></td>' +
                             '<td colspan="2" class="vehicleName"><a href="#" name="btnZoomTo">' + currentVehicle.name + '</a><input type="hidden" id="lonlat" value="' + currentVehicle.loc.x + "," + currentVehicle.loc.y + '" /></td>');
                tr1Content.appendTo(tr1);

                var tr2 = $('<tr class="vehicleTxt">');
                var motionStatusImg = currentVehicle.motion == 1?"Images/ball_green.png":"Images/ball_gray.png";
                var motionLabel = currentVehicle.motion == 1 ? "In Motion" : "Idle";
                var tr2Content = $('<td colspan="2"> <img alt="ball" src="' + motionStatusImg + '" /><span id="motionStatus" class="greenTxt">' + motionLabel + '</span></td>');
                tr2Content.appendTo(tr2);

                var tr3 = $('<tr class="vehicleTxt">');
                var isInFenceLabel = currentVehicle.isIn ? "<span class='redTxt'>In Restrict Area</span>" : "<span>Out Of Restrict Area</span>"
                var tr3Content = $('<td>Area:</td><td id="isInFence">' + isInFenceLabel + '</td>');
                tr3Content.appendTo(tr3);

                var tr4 = $('<tr class="vehicleTxt">');
                var tr4Content = $('<td>Speed:</td><td>' + currentVehicle.loc.s + ' mph</td>');
                tr4Content.appendTo(tr4);

                var tr5 = $('<tr class="vehicleTxt">');
                var tr5Content = $('<td>Duration:</td><td>' + currentVehicle.dur + ' min</td>');
                tr5Content.appendTo(tr5);

                var tr6 = $('<tr class="vehicleTxt">');
                var tr6Content = $('<td colspan="3">&nbsp;</td>');
                tr6Content.appendTo(tr6);

                tr1.appendTo(table);
                tr2.appendTo(table);
                tr3.appendTo(table);
                tr4.appendTo(table);
                tr5.appendTo(table);
                tr6.appendTo(table);
            }

            return table.html();
        }
    }
}