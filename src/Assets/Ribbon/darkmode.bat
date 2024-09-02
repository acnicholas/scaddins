rem For each .bmp file in this folder,
rem   invert the grayscale pixels only (white->black,
rem   lightgray->darkgray, darkgray->lightgray,
rem   black->white etc.)
rem   convert the black pixels to transparent,
rem   add 30 to each R,G,B value (necessary as our dark
rem   mode is not black - this is an arbitrary value to
rem   lighten the overall image),
rem   and save the result to _dark.png.
for %%f in (*.png) do (C:\Home\Andrew\bin\ImageMagick-7.1.1-31-portable-Q16-x64\convert -channel RGB -negate -transparent black -colorize 20,20,20 %%f %%~nf-dark.png)
rem for %%f in (*.png) do (C:\Home\Andrew\bin\ImageMagick-7.1.1-31-portable-Q16-x64\convert +negate -colorize 35,35,35  %%f %%~nf_dark.png)
pause