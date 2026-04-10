s = ""
a = 1
b = 0
for i=2,20,1 do
    b = a * i
    a = b
    s = s .. i .. ": " .. b .."\n"
    
end
return s