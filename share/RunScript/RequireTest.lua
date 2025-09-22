local localappdata = os.getenv("LOCALAPPDATA")
package.path = localappdata .. "\\Studio.SC\\SCaddins\\share\\RunScript\\?.lua;" .. package.path
require("HelloWorld")
require("HelloSCaddins")


