function drawCircle(doc, view, x, y, r)
            app = doc.Application
            point1 = XYZ(MillimetersToFeet(x), MillimetersToFeet(y), MillimetersToFeet(0))
            arc = Arc.Create(point1,MillimetersToFeet(r),0,360,XYZ(1, 0, 0),XYZ(0, 1, 0))
            detailCurve = doc.Create:NewDetailCurve(view, arc)
            arc:Dispose()
end

function drawLine(doc, view, x1, y1, x2, y2)
            app = doc.Application
            point1 = XYZ(MillimetersToFeet(x1), MillimetersToFeet(y1), MillimetersToFeet(0))
            point2 = XYZ(MillimetersToFeet(x2), MillimetersToFeet(y2), MillimetersToFeet(0))
            line = Line.CreateBound(point1, point2)
            detailCurve = doc.Create:NewDetailCurve(view, line)
            line:Dispose()
end


function MillimetersToFeet(lengthInMm)
    return lengthInMm / 304.8
end
