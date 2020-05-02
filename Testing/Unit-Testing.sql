CREATE DEFINER=`master431`@`%` PROCEDURE `unitTesting`()
BEGIN
/*
    The following tests all the stored procedures.  We will be seeding the inventory with the following:
    Game, Pack, ticket number, Game name, Price of ticket, id of person signed in
    The stored procedure is called seedNewGame which will insert an unactivated pack into the inventory
	The set procedure will setup 
*/ 

-- We must remove anything in the inventory for this game to start clean---
delete from tblTickets where game='1437';

-- We need to set the testing game pack and number along with todays date
set @sGame='1437';
set @sPack='900212';
set @sNbr='019';
set @sDate=DATE(curdate());

-- Call the stored procedure passing the game, pack, Will always be the highest number in pack, name of game, price, employee id 
call SIS.seedNewGame(@sGame, @sPack, @sNbr, '200 Million Cash Bonanza', 30, 'zzz');
-- Validation step.  We have entered a test user id 'zzz'  if it hit the database then this variable will be set
set @Emp_id = (Select Emp_id from tblTickets WHERE Game=@sGame AND Pack=@sPack AND Nbr=@sNbr AND Date=@sDate);
-- if the SIS.seedNewGame was successful then the next line set seedNewGameCheck will say Insertion Test Passed.
set @seedNewGameCheck = (SELECT IF(@Emp_id = 'zzz','Seeded New Game Test ---- Passed','Seeded New Game Test ---- Failed'));

-- Next step is to activate the seeded pack
-- When a new pack is activated the first ticket in the pack is scanned and will always be 000
-- We changed the employee to be qqq for validation
set @sNbr='000';
call SIS.activatePack(@sGame, @sPack, @sNbr, 'qqq');
-- Validation step.  We have entered a test user id 'qqq'  if it hit the database then this variable will be set
set @Emp_id = (Select Emp_id from tblTickets WHERE Game=@sGame AND Pack=@sPack AND Nbr=@sNbr AND Date=@sDate);
-- if the SIS.seedNewGame was successful then the next line set seedNewGameCheck will say Insertion Test Passed.
set @activatePackCheck = (SELECT IF(@Emp_id = 'qqq','Activate Test ---- Passed','Activate Test ---- Failed'));


-- Next step is to end the day.  We decided to sell 5 tickets from that pack (0 through 4).  Top ticket in bin is now 5.
set @sNbr='005';
call SIS.endDayEmp(@sGame, @sPack, @sNbr, 'qqq');
-- Validation step.  We have entered a test user id 'qqq'  if it hit the database then this variable will be set
set @Emp_id = (Select Emp_id from tblTickets WHERE Game=@sGame AND Pack=@sPack AND Nbr=@sNbr AND Date=@sDate);
-- if the SIS.seedNewGame was successful then the next line set seedNewGameCheck will say Insertion Test Passed.
set @endDayEmpCheck = (SELECT IF(@Emp_id = 'qqq','End Day Test ---- Passed','End Day Test ---- Failed'));


-- Next we never tested start a day.  We tested from the view of a new pack.  This tests if the pack was already activated.
-- The end day ticket for that game should match the start of the next day or someone stole a ticket.  Different employee 'rrr'
call SIS.startDayEmp(@sGame, @sPack, @sNbr, 'rrr');
-- Validation step.  We have entered a test user id 'qqq'  if it hit the database then this variable will be set
set @Emp_id = (Select Emp_id from tblTickets WHERE Game=@sGame AND Pack=@sPack AND Nbr=@sNbr AND Date=@sDate);
-- if the SIS.seedNewGame was successful then the next line set seedNewGameCheck will say Insertion Test Passed.
set @startDayEmpCheck2 = (SELECT IF(@Emp_id = 'rrr','Start Day Test ---- Passed','Start Day Test ---- Failed'));

-- Next step is to end the day.  We decided to sell another 5 tickets from that pack (4 through 9).  Top ticket in bin is now 10.
set @sNbr='010';
call SIS.endDayEmp(@sGame, @sPack, @sNbr, 'rrr');
-- Validation step.  We have entered a test user id 'qqq'  if it hit the database then this variable will be set
set @Emp_id = (Select Emp_id from tblTickets WHERE Game=@sGame AND Pack=@sPack AND Nbr=@sNbr AND Date=@sDate);
-- if the SIS.seedNewGame was successful then the next line set seedNewGameCheck will say Insertion Test Passed.
set @endDayEmpCheck2 = (SELECT IF(@Emp_id = 'rrr','End Day Test ---- Passed','End Day Test ---- Failed'));

-- Next step is to check counts daily which should be 10
-- This is the exact weeklyCounts stored procedure

DROP TEMPORARY TABLE IF EXISTS d1;
CREATE TEMPORARY TABLE d1 (Select Game, Pack, Nbr, LoadedPack, game as Total  FROM SIS.tblTickets WHERE Date = curdate() and Activated=true);
DROP TEMPORARY TABLE IF EXISTS d2;
CREATE TEMPORARY TABLE d2 (Select Pack, Game, MAX(Nbr) as End, MIN(Nbr) as Begin, Max(Nbr) - Min(Nbr) as Total, MAX(LoadedPack) as Loaded from d1 group by Game, Pack);
DROP TEMPORARY TABLE IF EXISTS d3;
CREATE TEMPORARY TABLE d3 (select if (Loaded=1, Total, sum(Total + 1)) as FinalTotal, Game, Pack from d2 group by Game, Pack );
set @dlytotal = (Select SUM(FinalTotal) from d3 where game ='1437');
set @dailyCountCheck = (SELECT IF(@dlytotal = 10,'Daily count test ---- Passed','Daily count test ---- Failed'));


-- Next step is to check counts weekly which should be 10

DROP TEMPORARY TABLE IF EXISTS w1;
CREATE TEMPORARY TABLE w1 (Select Game, Pack, Nbr, LoadedPack, game as Total  FROM SIS.tblTickets WHERE Activated=true AND Date >= (DATE_SUB(CURDATE(), INTERVAL 1 WEEK)));
DROP TEMPORARY TABLE IF EXISTS w2;
CREATE TEMPORARY TABLE w2 (Select Pack, Game, MAX(Nbr) as End, MIN(Nbr) as Begin, Max(Nbr) - Min(Nbr) as Total, MAX(LoadedPack) as Loaded from w1 group by Game, Pack);
DROP TEMPORARY TABLE IF EXISTS w3;
CREATE TEMPORARY TABLE w3 (select if (Loaded=1, Total, sum(Total + 1)) as FinalTotal, Game, Pack from w2 group by Game, Pack );
set @weeklytotal = (Select FinalTotal from w3 where game='1437');
set @WeeklyCountCheck = (SELECT IF(@weeklytotal = 10,'Weekly count test ---- Passed','Weekly count test ---- Failed'));

-- Next step is to check counts Monthy which should be 10
-- This is the exact monthlyCounts stored procedure

DROP TEMPORARY TABLE IF EXISTS m1;
CREATE TEMPORARY TABLE m1 (Select Game, Pack, Nbr, LoadedPack, game as Total  FROM SIS.tblTickets WHERE Activated=true AND Date >= (DATE_SUB(CURDATE(), INTERVAL 30 DAY)));
DROP TEMPORARY TABLE IF EXISTS m2;
CREATE TEMPORARY TABLE m2 (Select Pack, Game, MAX(Nbr) as End, MIN(Nbr) as Begin, Max(Nbr) - Min(Nbr) as Total, MAX(LoadedPack) as Loaded from m1 group by Game, Pack);
DROP TEMPORARY TABLE IF EXISTS m3;
CREATE TEMPORARY TABLE m3 (select if (Loaded=1, Total, sum(Total + 1)) as FinalTotal, Game, Pack from m2 group by Game, Pack );
set @monthlytotal = (Select FinalTotal from w3 where game='1437');
set @monthlyCountCheck = (SELECT IF(@monthlytotal = 10,'Monthly count test ---- Passed','Monthly count test ---- Failed'));

-- verify that emp10@gmail.com exists in the user table
set @email = (SELECT Email FROM tblUsers where Emp_id='31');
set @userValidationCheck = (SELECT IF(@email = 'emp10@gmail.com','User Validation test ---- Passed','User Validation test ---- Failed'));


-- Put all the variables we set above for all the tests with pass or fail into a tempory table to be displayed.
DROP TEMPORARY TABLE IF EXISTS t1;
CREATE TEMPORARY TABLE t1(TestDescription varchar(50));
insert INTO t1(select @seedNewGameCheck);
insert INTO t1(select @activatePackCheck);
insert INTO t1(select @endDayEmpCheck);
insert INTO t1(select @startDayEmpCheck2);
insert INTO t1(select @endDayEmpCheck2);
insert INTO t1(select @dailyCountCheck);
insert INTO t1(select @dailyCountCheck);
insert INTO t1(select @WeeklyCountCheck);
insert INTO t1(select @monthlyCountCheck);
insert INTO t1(select @userValidationCheck);

select * from t1;
END