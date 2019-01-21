<?php
	$con = mysqli_connect('localhost', 'root', null,'logs');
	if(!$con){
		die('Соединение с сервером не выполнено');
	}
	
	$type = $_POST["type"];
	$data = array();
	$data[0] = $type;
	
	if($type == 0){
		$sql = "SELECT MIN(DATE(DateTime)) as DateMin, MAX(DATE(DateTime)) as DateMax ";
		$sql .= "FROM action";
		$result = mysqli_query($con, $sql);
		$data[1] = mysqli_fetch_assoc($result);
		$sql = "SELECT * FROM category";
		$result = mysqli_query($con, $sql);
		while($row = mysqli_fetch_assoc($result)){
			$data[3][] = $row;
		}
	}
	else if($type == 1){
		$sql = "SELECT u.Country as Country, COUNT(*) as aCount ";
		$sql .= "FROM action a JOIN user u ON a.UserID = u.UserID ";
		$sql .= "WHERE DATE(a.DateTime) BETWEEN '".$_POST["from"]."' AND '".$_POST["to"]."' ";
		$sql .= "GROUP by u.Country ORDER by aCount DESC limit 10";
		$result = mysqli_query($con, $sql);
		$data[1][0][] = "Country";
		$data[1][0][] = "Количество действий";
		while($row = mysqli_fetch_row($result)){
			$data[1][] = $row;
		}
	}
	else if($type == 2){
		$sql = "SELECT u.Country as Country, COUNT(g.Name) as aCount ";
		$sql .= "FROM action a JOIN goods g ON a.ActionReference = g.GoodsID ";
		$sql .= "JOIN category c ON g.CategoryID = c.CategoryID JOIN user u ON a.UserID = u.UserID ";
		$sql .= "WHERE a.ActionType = 2 AND c.CategoryID = ".$_POST["ID"];
		$sql .= " GROUP by u.Country ORDER by aCount DESC limit 10";
		$result = mysqli_query($con, $sql);
		$data[1][0][] = "Country";
		$data[1][0][] = "Count";
		while($row = mysqli_fetch_row($result)){
			$data[1][] = $row;
		}
	}
	else if($type == 7){
		$sql = "SELECT t.uCount, COUNT(t.uCount) as countSUM FROM";
		$sql .= "(SELECT COUNT(a.UserID) as uCount FROM action a ";
		$sql .= "WHERE a.ActionType = 5 AND DATE(a.DateTime) BETWEEN '".$_POST["from"]."' AND '".$_POST["to"]."' ";
		$sql .= "GROUP by a.UserID) as t GROUP by t.uCount";
		$result = mysqli_query($con, $sql);
		$data[1][0][] = "Country";
		$data[1][0][] = "Пользователей";
		while($row = mysqli_fetch_row($result)){
			$data[1][] = $row;
		}
	}
	
	mysqli_close($con);
	echo json_encode($data, JSON_NUMERIC_CHECK);
?>