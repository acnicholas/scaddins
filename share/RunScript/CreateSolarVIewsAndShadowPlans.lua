package.path = "C:\\Program Files\\Studio.SC\\SCaddins\\share\\RunScript\\?.lua;" .. package.path
--below will create shadow plans and sun eye veiws at the defualt times.
--defualt times are hourly from 9am-3pm June 21
require("CreateSolarVIews")
require("CreateShadowPlans")