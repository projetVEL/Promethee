using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ButtonTrueFalse
{
    ///<summary>
    ///On crée ici un bouton TrueFalse afin de nous renvoyer son état. Ainsi suivant son état on pourra adopter un certain comportement
    ///On renverra un stateObject supplémentaire pendant les 0.5s qui suivent un changement d'état
    ///<summary>
    public partial class Form1 : Form
    {
        ///<summary>
        ///state : Etat actuel du bouton
        ///changed : Indique si il y a eu une modification de l'état du bouton
        ///changeDisplay : Indique si on a commencé à indiquer 
        ///</summary>
        public bool state = false, changed = false, changeDisplay = false;
        /// <summary>
        /// Nous permettra de déterminer si on renvoie ou non l'état de changement
        /// </summary>
        public DateTime lastChange;

        /// <summary>
        /// on initialise la fenêtre, rien d'exceptionnel
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// On charge la fenêtre, ainsi on crée son comportement
        /// D'abord on règle le bouton, sa couleur ainsi que son texte. De plus on met aussi le texte dans le label (amis daltoniens, nous pensons à vous)
        /// Ensuite on push les StateObjects afin d'en avoir toujours un, par défaut le bouton est sur false
        /// S'en suit juste l'attente de changements afin d'afficher si il y a eu un changement il a moins de 0.5 secondes
        /// </summary>
        /// <param name="sender">Présent de base</param>
        /// <param name="e">présent de base également</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            PackageHost.RegisterStateObjectLinks(this);
            PackageHost.RegisterMessageCallbacks(this);
            PackageHost.DeclarePackageDescriptor();
            button1.Text = "Push Me";
            button1.BackColor = Color.FromKnownColor(KnownColor.Red);
            label1.Text = "Off";
            PackageHost.PushStateObject("isOn", false);
            PackageHost.PushStateObject("changeState", false);

            Task.Factory.StartNew(() =>
            {
                var lastTime = DateTime.MinValue;
                while (PackageHost.IsRunning)
                {
                    if (DateTime.Now.Subtract(lastTime).TotalMilliseconds >= 1)
                    {
                        if (changed)
                        {
                            changed = false;
                            changeDisplay = true;
                            lastChange = DateTime.Now;
                            PackageHost.PushStateObject("changeState", true);
                        }
                        if (DateTime.Now.Subtract(lastChange).TotalMilliseconds > 500 && changeDisplay)
                        {
                            changeDisplay = false;
                            PackageHost.PushStateObject("changeState", false);

                        }

                        lastTime = DateTime.Now;
                    }
                    Thread.Sleep(1);
                }

            });

            this.Text = string.Format("IsRunning: {0} - IsConnected: {1} - IsStandAlone: {2}", PackageHost.IsRunning, PackageHost.IsConnected, PackageHost.IsStandAlone);
            PackageHost.WriteInfo("I'm running !");
        }



        /// <summary>
        /// Si on clique sur le bouton le changera d'état, on changera sa couleur  et on changera le texte du label
        /// De plus on push le StateObject avec le nouvel état 
        /// Ensuite on met <c> changed </c> en true afin de pouvoir renvoyer le SO de modification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (state)
            {
                state = false;
                button1.BackColor = Color.FromKnownColor(KnownColor.Red);
                label1.Text = "Off";
                PackageHost.PushStateObject("isOn", state);
                changed = true;

            }
            else
            {
                state = true;
                button1.BackColor = Color.FromKnownColor(KnownColor.Green);
                label1.Text = "On";
                PackageHost.PushStateObject("isOn", state);
                changed = true;
            }
            PackageHost.PushStateObject("isOn", state);
        }
        /// <summary>
        /// On décrit ici le message Callback si on préfère modifier l'état du bouton par le biai d'un Callback plutôt que par un clic sur le bouton
        /// Le comportement est le même que pour le clique, on gère simplement le fait que si le bouton est sur un état, et qu'on demande ce même état, on ne change rien
        /// </summary>
        /// <param name="set"> Etat que l'on souhaite donner au bouton</param>
        [MessageCallback(Key = "set")]
        void set(bool set)
        {
            if (state)
            {
                if (!set)
                {
                    state = false;
                    button1.BackColor = Color.FromKnownColor(KnownColor.Red);
                    PackageHost.PushStateObject("isOn", state);
                    label1.Text = "Off";
                    changed = true;
                }

            }
            else
            {
                if (set)
                {
                    state = true;
                    button1.BackColor = Color.FromKnownColor(KnownColor.Green);
                    PackageHost.PushStateObject("isOn", state);
                    label1.Text = "On";
                    changed = true;
                }
            }

        }
    }
}

