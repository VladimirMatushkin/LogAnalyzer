USE `logs`;
LOAD DATA INFILE $$PATH$$category.txt' INTO TABLE `Category` 
FIELDS TERMINATED BY ',';

LOAD DATA INFILE $$PATH$$goods.txt' INTO TABLE `Goods`
FIELDS TERMINATED BY ',';

LOAD DATA INFILE $$PATH$$user.txt' INTO TABLE `User`
FIELDS TERMINATED BY ',' (`User`.`UserID`, `User`.`IP`);

LOAD DATA INFILE $$PATH$$cart.txt' INTO TABLE `Cart` 
FIELDS TERMINATED BY ',';

LOAD DATA INFILE $$PATH$$cartGoods.txt' INTO TABLE `CartGoods` 
FIELDS TERMINATED BY ',';

LOAD DATA INFILE $$PATH$$actions.txt' INTO TABLE `Action` 
FIELDS TERMINATED BY ',' (`Action`.`UserID`,`Action`.`ActionType`,`Action`.`ActionReference`,`Action`.`DateTime`);