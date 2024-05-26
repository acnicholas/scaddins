import ('System.Windows.Forms', 'System.Windows.Forms')

saveFileDialog1 = SaveFileDialog();
dr = saveFileDialog1:ShowDialog();

if dr == DialogResult.OK then 
	return saveFileDialog1.FileName
else
	return "error"
end
