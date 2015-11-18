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

        /// <summary>
        /// Required designer variable
        /// </summary>
        private System.ComponentModel.Container components = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSearchAll_Click_1(object sender, EventArgs e)
        {
            //GUI CHANGE - Clears the Datagrid view for incoming data.
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            //FUNCTIONAL CHANGE - Change the button to searching form.
            btnSearchAll.Text = "Searching...";
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            //FUNCTION - Searches the given directory from the form with the .ojn file extension
            //           and displays information regarding the .ojn file.
            DirSearch(DirectoryBox.Text,1);

            //FUNCTIONAL CHANGE - RE-Change the button back to it's orginal form.
            btnSearchAll.Text = "Search All";
            this.Cursor = Cursors.Default;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //GUI CHANGE - Clears the Datagrid view for incoming data.
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            //FUNCTIONAL CHANGE - Change the button to searching form.
            btnSearch.Text = "Searching...";
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            //FUNCTION - Searches the given directory with level parameters from 
            //           the form with the .ojn file extension and displays information
            //           regarding the .ojn file.
            DirSearch(DirectoryBox.Text, 2);

            //FUNCTIONAL CHANGE - RE-Change the button back to it's orginal form.
            btnSearch.Text = "Search";
            this.Cursor = Cursors.Default;
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
        { }

        /// <summary>
        /// 
        /// <SEARCH ALL FUNCTION>
        /// 
        /// -----------------------------------------------------------------------------------------------------------------
        /// IF OPTION_ == 1
        /// The searching algorithm function for non-specific searches desired by the user, to search for the all .ojn files.
        /// -----------------------------------------------------------------------------------------------------------------
        /// IF OPTION_ == 2
        /// The searching algorithm function for specific searches desired by the user, to search for the .ojn files.
        /// You must include a parameter of a search number in either textbox1,textbox2 or textbox3 to be able to use this function.
        /// 
        /// TO DO: Insert pre-loader directory changer, fix the searching algorithm based on different textbox searches.
        /// 
        /// </summary>
        void DirSearch(string sDir, int option_)
        {

            //
            // The simplest overload of MessageBox.Show. [1]
            //
            //MessageBox.Show("Dot Net Perls is awesome.");

            if(!sDir.Equals(""))
            {
                //Check and see if the directory has those files contained.
                //...
                //end

                string assemblypath = Environment.CurrentDirectory;
                string[] o2jamojnfiles = Directory.GetFiles(sDir, "*.ojn", SearchOption.AllDirectories);
                int counter = 0;

                try
                {
                    foreach (string o2File in o2jamojnfiles)
                    {
                        counter += 1;
                        #region filenames/locations
                        //string filedir = Path.GetDirectoryName(o2File);
                        string filedir = sDir;
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
                        headerojn[20] = 99;

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

                        if (option_ == 1)
                        {
                            string[] row = new string[] { convertCounter, TotalLevel, Path.GetFileName(o2File), Title, Artist, Notecharter };
                            dataGridView1.Rows.Add(row);
                        }
                        else
                        {

                        }

                        //close stream reader and clean up read
                        fs.Close();
                        fs.Dispose();
                        header.Close();
                        header.Dispose();
                        headerojn = null;
                        #endregion

                        #endregion
                    }
                }
                catch (System.Exception excpt)
                {
                    Console.WriteLine(excpt.Message);
                }
                /// </summary>

            }
            else { MessageBox.Show("There is no valid search parameters in the search box!"); }
            

        }

    }//DIRECTORY SEARCH ALL - END

}//MAIN FORM - END

