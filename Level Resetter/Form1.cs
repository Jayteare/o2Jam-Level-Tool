using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Level_Resetter
{
    public partial class Form1 : Form
    {

        internal System.Windows.Forms.Button btnSearch;
        internal System.Windows.Forms.Label lblDirectory;
        internal System.Windows.Forms.ComboBox cboDirectory;

        /// <summary>
        /// Required designer variable
        /// </summary>
        private System.ComponentModel.Container components = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //GUI CHANGE - Clears the Datagrid view for incoming data.
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            //FUNCTIONAL CHANGE - Clears the display textbox.
            cboDirectory.Enabled = false;

            //FUNCTIONAL CHANGE - Change the button to searching form.
            btnSearch.Text = "Searching...";
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            //FUNCTION - Searches the given directory with level parameters from 
            //           the form with the .ojn file extension and displays information
            //           regarding the .ojn file.
            DirSearch(cboDirectory.Text);

            //FUNCTIONAL CHANGE - RE-Change the button back to it's orginal form.
            btnSearch.Text = "Search";
            this.Cursor = Cursors.Default;
            cboDirectory.Enabled = true;
        }

        private void btnSearchAll_Click_1(object sender, EventArgs e)
        {
            //GUI CHANGE - Clears the Datagrid view for incoming data.
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            //FUNCTIONAL CHANGE - Clears the display textbox.
            cboDirectory.Enabled = false;

            //FUNCTIONAL CHANGE - Change the button to searching form.
            btnSearchAll.Text = "Searching...";
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            //FUNCTION - Searches the given directory from the form with the .ojn file extension
            //           and displays information regarding the .ojn file.
            DirSearchAll(cboDirectory.Text);

            //FUNCTIONAL CHANGE - RE-Change the button back to it's orginal form.
            btnSearchAll.Text = "Search All";
            this.Cursor = Cursors.Default;
            cboDirectory.Enabled = true;
        }

        /// <summary>
        /// 
        /// <LOADER FORM GUI FUNCTION>
        /// 
        /// The PRE-LOADER FUNCTION for the program, it will load the desired placeholders with desired input.
        /// 
        /// NOTE: Computer Search Parameter can be added.
        /// </summary>
        private void Form1_Load(object sender, System.EventArgs e)
        {
            cboDirectory.Items.Clear();
            foreach (string s in Directory.GetLogicalDrives())
            {
                cboDirectory.Items.Add(s);
            }
            cboDirectory.Text = "C:\\";
        }

        /// <summary>
        /// 
        /// <DIRECTED SEARCH FUNCTION>
        /// 
        /// The searching algorithm function for specific searches desired by the user, to search for the .ojn files.
        /// You must include a parameter of a search number in either textbox1,textbox2 or textbox3 to be able to use this function.
        /// 
        /// TO DO: Insert pre-loader directory changer, fix the searching algorithm based on different textbox searches.
        /// </summary>
        void DirSearch(string sDir)
        {

            string assemblypath    = Environment.CurrentDirectory;
            string[] o2jamojnfiles = Directory.GetFiles("F:\\Games\\O2China English\\O2Emu\\Music\\", "*.ojn", SearchOption.AllDirectories);
            int counter = 0;

            try
            {
                foreach (string o2File in o2jamojnfiles)
                {
                    counter += 1;
                    #region filenames/locations
                    //string filedir = Path.GetDirectoryName(o2File);
                    string filedir = "F:\\Games\\O2China English\\O2Emu\\Music\\";
                    string ojmname = Path.GetFileNameWithoutExtension(o2File) + ".ojm";
                    string ojnname = Path.GetFileNameWithoutExtension(o2File);
                    string ojmfile = filedir + ojmname;
                    string[] files = Directory.GetFiles(filedir);
                        
                    #region .ojn file header
                    //read file header of .ojn
                    FileStream fs = File.Open(o2File, FileMode.Open, FileAccess.Read);
                    BinaryReader header = new BinaryReader(fs);
                    header.BaseStream.Position = 0;
                    byte[] headerojn = header.ReadBytes(0x12C);
                    //bpm
                    float o2jamBPM = System.BitConverter.ToSingle(headerojn.Skip(16).Take(4).ToArray(), 0);
                    //difficulties
                    int lvleasy   = BitConverter.ToUInt16(headerojn.Skip(20).Take(2).ToArray(), 0);
                    int lvlnormal = BitConverter.ToUInt16(headerojn.Skip(22).Take(2).ToArray(), 0);
                    int lvlhard   = BitConverter.ToUInt16(headerojn.Skip(24).Take(2).ToArray(), 0);
                    //notes per difficulty
                    int noteseasy   = BitConverter.ToInt32(headerojn.Skip(40).Take(4).ToArray(), 0);
                    int notesnormal = BitConverter.ToInt32(headerojn.Skip(44).Take(4).ToArray(), 0);
                    int noteshard   = BitConverter.ToInt32(headerojn.Skip(48).Take(4).ToArray(), 0);
                    //title artist notecharter
                    string Title  = "";
                    string Artist = "";
                    string Notecharter = "";
                    //there are 2 encoding types, see if you can find a workaround for this
                    //chinese
                    Title  = Encoding.GetEncoding(54936).GetString(headerojn.Skip(108).Take(64).ToArray());
                    Artist = Encoding.GetEncoding(54936).GetString(headerojn.Skip(172).Take(32).ToArray());
                    Notecharter = Encoding.GetEncoding(54936).GetString(headerojn.Skip(204).Take(32).ToArray());

                    //String converts for the display.
                    string EpicMix = Title + "," + Artist + "," + Notecharter;
                    string EXLevel = lvleasy.ToString();
                    string NXLevel = lvlnormal.ToString();
                    string HXLevel = lvlhard.ToString();
                    int excompareMain = Int32.Parse(EXLevel);
                    int excompareBox1 = Int32.Parse(textBox2.Text);
                    if(textBox2.Text != null)
                    {
                        if(excompareMain <= excompareBox1)
                        {
                            string convertCounter = counter.ToString();
                            string TotalLevel = EXLevel + "/" + NXLevel + "/" + HXLevel;
                            //Datagrid view implementation for the o2jam list.
                            string[] row = new string[] { convertCounter, TotalLevel, Path.GetFileName(o2File), Title, Artist, Notecharter };
                            dataGridView1.Rows.Add(row);
                        }


                    }
                        //NEED TO FIX BEYOND
                    else if(textBox3.Text != null)
                    {
                        //MessageBox - Displays the Target path does not exist.
                        MessageBox.Show("The target path does not exist.");

                        int nxcompareMain = Int32.Parse(NXLevel);
                        int nxcompareBox1 = Int32.Parse(textBox3.Text);
                        if (nxcompareMain <= nxcompareBox1)
                        {
                            string convertCounter = counter.ToString();
                            string TotalLevel = EXLevel + "/" + NXLevel + "/" + HXLevel;
                            //Datagrid view implementation for the o2jam list.
                            string[] row = new string[] { convertCounter, TotalLevel, Path.GetFileName(o2File), Title, Artist, Notecharter };
                            dataGridView1.Rows.Add(row);
                        }
                    }

                    //Korean
                    //Title = Encoding.GetEncoding(51949).GetString(headerojn.Skip(108).Take(32).ToArray()); // Output byte array in text
                    //Artist = Encoding.GetEncoding(51949).GetString(headerojn.Skip(172).Take(32).ToArray()); // Output byte array in text
                    //Notecharter = Encoding.GetEncoding(51949).GetString(headerojn.Skip(204).Take(32).ToArray()); // Output byte array in text
                    //background/loading image position & size
                    int jpegsize   = BitConverter.ToInt32(headerojn.Skip(268).Take(4).ToArray(), 0);
                    int jpegoffset = BitConverter.ToInt32(headerojn.Skip(296).Take(4).ToArray(), 0);
                    //difficulty byte reading offsets
                    int easyoffset   = BitConverter.ToInt32(headerojn.Skip(284).Take(4).ToArray(), 0);
                    int normaloffset = BitConverter.ToInt32(headerojn.Skip(288).Take(4).ToArray(), 0);
                    int hardoffset   = BitConverter.ToInt32(headerojn.Skip(292).Take(4).ToArray(), 0);
                    //close stream reader and clean up read
                    fs.Close();
                    fs.Dispose();
                    header.Close();
                    header.Dispose();
                    headerojn = null;
                    #endregion

                    #endregion
;                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            /// </summary>


        }//DIRECTORY SEARCH - END

        /// <summary>
        /// 
        /// <SEARCH ALL FUNCTION>
        /// 
        /// The searching algorithm function for specific searches desired by the user, to search for the .ojn files.
        /// You must include a parameter of a search number in either textbox1,textbox2 or textbox3 to be able to use this function.
        /// 
        /// </summary>
        void DirSearchAll(string sDir)
        {

            //
            // The simplest overload of MessageBox.Show. [1]
            //
            //MessageBox.Show("Dot Net Perls is awesome.");

            string assemblypath = Environment.CurrentDirectory;
            string[] o2jamojnfiles = Directory.GetFiles("F:\\Games\\O2China English\\O2Emu\\Music\\", "*.ojn", SearchOption.AllDirectories);
            int counter = 0;

            try
            {
                foreach (string o2File in o2jamojnfiles)
                {
                    counter += 1;
                    #region filenames/locations
                    //string filedir = Path.GetDirectoryName(o2File);
                    string filedir = "F:\\Games\\O2China English\\O2Emu\\Music\\";
                    string ojmname = Path.GetFileNameWithoutExtension(o2File) + ".ojm";
                    string ojnname = Path.GetFileNameWithoutExtension(o2File);
                    string ojmfile = filedir + ojmname;
                    string[] files = Directory.GetFiles(filedir);

                    #region .ojn file header
                    //read file header of .ojn
                    FileStream fs = File.Open(o2File, FileMode.Open, FileAccess.Read);
                    BinaryReader header = new BinaryReader(fs);
                    header.BaseStream.Position = 0;
                    byte[] headerojn = header.ReadBytes(0x12C);
                    //bpm
                    float o2jamBPM = System.BitConverter.ToSingle(headerojn.Skip(16).Take(4).ToArray(), 0);
                    //difficulties
                    int lvleasy = BitConverter.ToUInt16(headerojn.Skip(20).Take(2).ToArray(), 0);
                    int lvlnormal = BitConverter.ToUInt16(headerojn.Skip(22).Take(2).ToArray(), 0);
                    int lvlhard = BitConverter.ToUInt16(headerojn.Skip(24).Take(2).ToArray(), 0);
                    //notes per difficulty
                    int noteseasy = BitConverter.ToInt32(headerojn.Skip(40).Take(4).ToArray(), 0);
                    int notesnormal = BitConverter.ToInt32(headerojn.Skip(44).Take(4).ToArray(), 0);
                    int noteshard = BitConverter.ToInt32(headerojn.Skip(48).Take(4).ToArray(), 0);
                    //title artist notecharter
                    string Title = "";
                    string Artist = "";
                    string Notecharter = "";
                    //there are 2 encoding types, see if you can find a workaround for this
                    //chinese
                    Title = Encoding.GetEncoding(54936).GetString(headerojn.Skip(108).Take(64).ToArray());
                    Artist = Encoding.GetEncoding(54936).GetString(headerojn.Skip(172).Take(32).ToArray());
                    Notecharter = Encoding.GetEncoding(54936).GetString(headerojn.Skip(204).Take(32).ToArray());

                    //String converts for the display.
                    string EpicMix = Title + "," + Artist + "," + Notecharter;
                    string EXLevel = lvleasy.ToString();
                    string NXLevel = lvlnormal.ToString();
                    string HXLevel = lvlhard.ToString();
                    string convertCounter = counter.ToString();
                    string TotalLevel = EXLevel + "/" + NXLevel + "/" + HXLevel;
                    //Datagrid view implementation for the o2jam list.
                    string[] row = new string[] { convertCounter, TotalLevel, Path.GetFileName(o2File), Title, Artist, Notecharter };
                    dataGridView1.Rows.Add(row);


                    //Korean
                    //Title = Encoding.GetEncoding(51949).GetString(headerojn.Skip(108).Take(32).ToArray()); // Output byte array in text
                    //Artist = Encoding.GetEncoding(51949).GetString(headerojn.Skip(172).Take(32).ToArray()); // Output byte array in text
                    //Notecharter = Encoding.GetEncoding(51949).GetString(headerojn.Skip(204).Take(32).ToArray()); // Output byte array in text
                    //background/loading image position & size
                    int jpegsize = BitConverter.ToInt32(headerojn.Skip(268).Take(4).ToArray(), 0);
                    int jpegoffset = BitConverter.ToInt32(headerojn.Skip(296).Take(4).ToArray(), 0);
                    //difficulty byte reading offsets
                    int easyoffset = BitConverter.ToInt32(headerojn.Skip(284).Take(4).ToArray(), 0);
                    int normaloffset = BitConverter.ToInt32(headerojn.Skip(288).Take(4).ToArray(), 0);
                    int hardoffset = BitConverter.ToInt32(headerojn.Skip(292).Take(4).ToArray(), 0);
                    //close stream reader and clean up read
                    fs.Close();
                    fs.Dispose();
                    header.Close();
                    header.Dispose();
                    headerojn = null;
                    #endregion

                    #endregion
                    ;
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            /// </summary>

        }

        /// <summary>
        /// 
        /// <COPY BUTTON FUNCTION>
        /// 
        /// The searching algorithm function for specific searches desired by the user, to search for the .ojn files.
        /// You must include a parameter of a search number in either textbox1,textbox2 or textbox3 to be able to use this function.
        /// 
        /// </summary>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            string fileName = "o2ma1001.ojn";
            string sourcePath = @"F:\\Games\\O2China English\\O2Emu\\Music\\";
            string targetPath = @"F:\\Games\\O2China English\\Test Folder";

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!System.IO.Directory.Exists(targetPath))
            {
                //Optional Folder Creation.
                //System.IO.Directory.CreateDirectory(targetPath);
                
                //MessageBox - Displays the Target path does not exist.
                MessageBox.Show("The target path does not exist.");

            }

            // To copy a file to another location and 
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourceFile, destFile, true);


            // To copy all the files in one directory to another directory.
            // Get the files in the source folder. (To recursively iterate through
            // all subfolders under the current directory, see
            // "How to: Iterate Through a Directory Tree.")
            // Note: Check for target path was performed previously
            //       in this code example.
            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string o2File in files)
                {
                    //#region .ojn file header
                    //read file header of .ojn
                    FileStream fs = File.Open(o2File, FileMode.Open, FileAccess.Read);
                    BinaryReader header = new BinaryReader(fs);
                    header.BaseStream.Position = 0;
                    byte[] headerojn = header.ReadBytes(0x12C);
                    //bpm
                    float o2jamBPM = System.BitConverter.ToSingle(headerojn.Skip(16).Take(4).ToArray(), 0);
                    //difficulties
                    int lvleasy = BitConverter.ToUInt16(headerojn.Skip(20).Take(2).ToArray(), 0);
                    int lvlnormal = BitConverter.ToUInt16(headerojn.Skip(22).Take(2).ToArray(), 0);
                    int lvlhard = BitConverter.ToUInt16(headerojn.Skip(24).Take(2).ToArray(), 0);

                    //title artist notecharter
                    string Title = "";
                    string Artist = "";
                    string Notecharter = "";
                    //there are 2 encoding types, see if you can find a workaround for this
                    //chinese
                    Title = Encoding.GetEncoding(54936).GetString(headerojn.Skip(108).Take(64).ToArray());
                    Artist = Encoding.GetEncoding(54936).GetString(headerojn.Skip(172).Take(32).ToArray());
                    Notecharter = Encoding.GetEncoding(54936).GetString(headerojn.Skip(204).Take(32).ToArray());

                    //String converts for the display.
                    string EpicMix = Title + "," + Artist + "," + Notecharter;
                    string EXLevel = lvleasy.ToString();
                    string NXLevel = lvlnormal.ToString();
                    string HXLevel = lvlhard.ToString();
                    int excompareMain = Int32.Parse(EXLevel);
                    int excompareBox1 = Int32.Parse(textBox2.Text);
                    if (textBox2.Text != null)
                    {
                        if (excompareMain <= excompareBox1)
                        {

                            fileName = System.IO.Path.GetFileName(o2File);
                            //MessageBox - Displays the Target path does not exist.
                            MessageBox.Show(fileName);
                            destFile = System.IO.Path.Combine(targetPath, fileName);
                            FileInfo file_info = new FileInfo(o2File);
                            this.Close();
                            File.Copy(o2File, destFile);
                            //System.IO.File.Copy(o2File, destFile);
                        }


                    }
                    // Use static Path methods to extract only the file name from the path.

                }
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }

            // Keep console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

    }//DIRECTORY SEARCH ALL - END

}//MAIN FORM - END

