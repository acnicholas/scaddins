s = ""
a = 0
b = 1
c = 0
s = s .. a .."\n"
s = s .. b .."\n"
for i=0,100,1 do
    c = a + b
    a = b
    b = c
    s = s .. c .."\n"
    
end
return s

-- 0, 1, 1, 2, 3, 5, 8, 13, 21, 34
