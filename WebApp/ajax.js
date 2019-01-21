var xmlHttp;
var options1 = {
	title: 'Топ 10 стран',
	width: 1000,
    height: 500,
	chartArea: {'width': '70%', 'height': '80%'}
};
var options2 = {
	title: 'Покупок одним пользователем',
	width: 900,
    height: 400,
	chartArea: {'width': '65%', 'height': '80%'},
	logScale: true,
	hAxis: {
        title: 'Количество покупок одним пользователем'
    },
    vAxis: {
		title: 'Пользователей'
    }
};

function createXMLHttpRequest(){
	if (window.XMLHttpRequest) {
        xmlHttp = new XMLHttpRequest();
    } else {
        xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlHttp.onreadystatechange = handleRequestStateChange;
}

function handleRequestStateChange(){
    if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
        var response = JSON.parse(xmlHttp.responseText);
		console.log(response);
		if(response[0] == 0)
			ServerResponse0(response);
		else if(response[0] == 1)
			ServerResponse1(response[1]);
		else if(response[0] == 2)
			ServerResponse2(response[1]);
		else if(response[0] == 7)
			ServerResponse7(response[1]);
    }
}

function ServerResponse0(response){
	var date = document.querySelectorAll("input[type=date]");
	var dMin = response[1].DateMin;
	var dMax = response[1].DateMax;
	for(var i = 0; i < date.length; i++){
		date[i].min = dMin;
		date[i].max = dMax;
		date[i].value = i%2 ? dMax : dMin;
	}
	
	var arr = response[3];
	var sel = document.getElementById('r2Category');
	for(var i = 0; i < arr.length; i++){
		var opt = document.createElement("option");
		opt.id = arr[i].CategoryID;
		opt.value = arr[i].Name;
		opt.text = arr[i].Name;
		sel.appendChild(opt);
	}
}

function ServerResponse1(response){
	var data = google.visualization.arrayToDataTable(response);

	var container = document.getElementById('r1PieChart');
    var chart = new google.visualization.ColumnChart(container);
	chart.draw(data, options1);
}

function ServerResponse2(response){
	var data = google.visualization.arrayToDataTable(response);

	var container = document.getElementById('r2PieChart');
    var chart = new google.visualization.PieChart(container);
	chart.draw(data, options1);
}

function ServerResponse7(response){
	console.log(response);
	for(var i = 0; i < response.length; i++){
		response[i][0] = response[i][0].toString();
	}
	var data = google.visualization.arrayToDataTable(response);
	
	var container = document.getElementById('r7PieChart');
    //var chart = new google.visualization.PieChart(container);
	var chart = new google.visualization.ColumnChart(container);
	chart.draw(data, options2);
}

function sendRequest(type){
	xmlHttp.open("POST", "query.php", true);
	xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
	if(type == 0){
		xmlHttp.send("type="+type);
	}
	else if(type == 1){
		var dFrom = document.getElementById("r1DFrom").value;
		var dTo = document.getElementById("r1DTo").value;
		xmlHttp.send("type="+type+"&from="+dFrom+"&to="+dTo);
	}
	else if(type == 2){
		var c = document.getElementById("r2Category");
		var cID = c[c.selectedIndex].id;
		xmlHttp.send("type="+type+"&ID="+cID);
	}
	else if(type == 7){
		var dFrom = document.getElementById("r7DFrom").value;
		var dTo = document.getElementById("r7DTo").value;
		xmlHttp.send("type="+type+"&from="+dFrom+"&to="+dTo);
	}
}

function Init(){
	createXMLHttpRequest();
	sendRequest(0);
}