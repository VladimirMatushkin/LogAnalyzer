CREATE DATABASE IF NOT EXISTS `logs` DEFAULT CHARACTER SET utf8 COLLATE utf8_bin;

USE `logs`;

DROP TABLE IF EXISTS `Action`;
DROP TABLE IF EXISTS `CartGoods`;
DROP TABLE IF EXISTS `Cart`;
DROP TABLE IF EXISTS `User`;
DROP TABLE IF EXISTS `Goods`;
DROP TABLE IF EXISTS `Category`;

CREATE TABLE `Category` (
    `CategoryID` int  NOT NULL ,
    `Name` varchar(50)  NOT NULL ,
    PRIMARY KEY (
        `CategoryID`
    ),
    CONSTRAINT `uc_Category_Name` UNIQUE (
        `Name`
    )
);

CREATE TABLE `Goods` (
    `GoodsID` int  NOT NULL ,
    `CategoryID` int  NOT NULL ,
    `Name` varchar(50)  NOT NULL ,
    PRIMARY KEY (
        `GoodsID`
    )
);

ALTER TABLE `Goods` ADD CONSTRAINT `fk_Goods_CategoryID` FOREIGN KEY(`CategoryID`)
REFERENCES `Category` (`CategoryID`);

CREATE TABLE `User` (
    `UserID` bigint  NOT NULL ,
    `IP` int unsigned  NOT NULL ,
    `Country` nvarchar(64) DEFAULT '-' ,
    PRIMARY KEY (
        `UserID`
    )
);

CREATE TABLE `Action` (
    `ActionID` int  NOT NULL AUTO_INCREMENT,
    `UserID` bigint NOT NULL ,
    `ActionType` int  NOT NULL ,
    `ActionReference` int  NULL ,
    `DateTime` datetime  NOT NULL ,
    PRIMARY KEY (
        `ActionID`
    )
);

ALTER TABLE `Action` ADD CONSTRAINT `fk_Action_UserID` FOREIGN KEY(`UserID`)
REFERENCES `User` (`UserID`);

CREATE TABLE `Cart` (
    `CartID` int  NOT NULL ,
    `UserID` bigint NOT NULL ,
    `Paid` tinyint(1) NOT NULL ,
    PRIMARY KEY (
        `CartID`
    )
);

ALTER TABLE `Cart` ADD CONSTRAINT `fk_Cart_UserID` FOREIGN KEY(`UserID`)
REFERENCES `User` (`UserID`);

CREATE TABLE `CartGoods` (
    `CartGoodsID` int  NOT NULL,
    `CartID` int  NOT NULL ,
    `GoodsID` int  NOT NULL ,
    `Amount` int  NOT NULL ,
    PRIMARY KEY (
        `CartGoodsID`
    )
);

ALTER TABLE `CartGoods` ADD CONSTRAINT `fk_CartGoods_CartID` FOREIGN KEY(`CartID`)
REFERENCES `Cart` (`CartID`);

ALTER TABLE `CartGoods` ADD CONSTRAINT `fk_CartGoods_GoodsID` FOREIGN KEY(`GoodsID`)
REFERENCES `Goods` (`GoodsID`);