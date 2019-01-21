CREATE DATABASE ip2location;
USE ip2location;
CREATE TABLE `ip2location_db1`(
	`ip_from` INT(10) UNSIGNED,
	`ip_to` INT(10) UNSIGNED,
	`country_code` CHAR(2),
	`country_name` VARCHAR(64),
	INDEX `idx_ip_from` (`ip_from`),
	INDEX `idx_ip_to` (`ip_to`),
	INDEX `idx_ip_from_to` (`ip_from`, `ip_to`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

LOAD DATA LOCAL
	INFILE 'IP2LOCATION-LITE-DB1.CSV'
INTO TABLE
	`ip2location_db1`
FIELDS TERMINATED BY ','
ENCLOSED BY '"'
LINES TERMINATED BY '\r\n'
IGNORE 0 LINES;