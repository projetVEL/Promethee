using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DisplaySomething
{
    public partial class Form1 : Form
    {/// <summary>
    /// On fait une fenêtre qui permettra d'afficher des messages, ainsi on peut renvoyer des informations au client sans qu'il ait besoin de passer par la console.
    /// </summary>
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            PackageHost.RegisterStateObjectLinks(this);
            PackageHost.RegisterMessageCallbacks(this);
            PackageHost.DeclarePackageDescriptor();
            label1.Font = new Font(label1.Font.FontFamily, 32);
            PackageHost.PushStateObject("TexteAffiché", "label1");
            PackageHost.PushStateObject("CouleurTexte", label1.ForeColor.ToArgb().ToString());
            PackageHost.PushStateObject("TaillePolice", label1.Font.Size);
            PackageHost.PushStateObject("PoliceDeCaractere", label1.Font.FontFamily.ToString());
            label1.AutoSize = false;
            label1.Size = new System.Drawing.Size(this.Height, this.Width);
            this.Text = string.Format("IsRunning: {0} - IsConnected: {1} - IsStandAlone: {2}", PackageHost.IsRunning, PackageHost.IsConnected, PackageHost.IsStandAlone);
            PackageHost.WriteInfo("I'm running !");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Change le texte à afficher dans le label
        /// </summary>
        /// <param name="texte">Nouveau Texte à afficher</param>
        [MessageCallback(Key = "AfficherTexte")]
        void DisplayText(string texte)
        {
            label1.Text = texte;
            PackageHost.PushStateObject("TexteAffiché", texte);
            label1.Size = new System.Drawing.Size(this.Height, this.Width);
        }
        
        /// <summary>
        /// Change la couleur du texte affiché
        /// </summary>
        /// <param name="a">Alpha (transparence)</param>
        /// <param name="r">Rouge</param>
        /// <param name="g">Vert</param>
        /// <param name="b">Bleu</param>
        [MessageCallback(Key = "ChangerCouleur")]
        void ChangeColor(int a, int r, int g, int b)
        {
            try
            {
                label1.ForeColor = Color.FromArgb(a, r, g, b);
            }catch(Exception e)
            {
                PackageHost.WriteInfo("Merci de mettre 4 entiers de sorte à faire une couleur ARGB");
                PackageHost.PushStateObject("CouleurTexte", label1.ForeColor.ToArgb().ToString());
            }
        }
        /// <summary>
        /// Change la taille de la police
        /// </summary>
        /// <param name="fontSize">La taille souhaitée pour la police</param>
        [MessageCallback (Key = "ChangerTaillePolice")]
        void ChangeFontSize(int fontSize)
        {
            try
            {
                label1.Font = new Font(label1.Font.FontFamily, fontSize);
                PackageHost.PushStateObject("TaillePolice", label1.Font.Size);

                label1.Size = new System.Drawing.Size(this.Height, this.Width);
            }
            catch(Exception e)
            {
                PackageHost.WriteInfo("Mettre un entier comme taille de caractère");
            }
        }
        /// <summary>
        /// Change la police d'écriture
        /// </summary>
        /// <param name="font">Nouvelle Police  (respecter les espaces et la casse)</param>
        [MessageCallback (Key ="ChangerPolice")]
        void changeFont(string font)
        {
            try
            {
                label1.Font = new Font(font, label1.Font.Size);
                PackageHost.PushStateObject("PoliceDeCaractere", label1.Font.FontFamily.ToString());

                label1.Size = new System.Drawing.Size(this.Height, this.Width);
            }
            catch(Exception e)
            {
                PackageHost.WriteInfo("Merci d'orthographier correctement la police telle qu'elle l'est dans votre dossier Font sur Windows");
            }
        }
        /// <summary>
        /// Change la police et la taille en même temps (Pour ne pas avoir à faire 2 callbacks en même temps)
        /// </summary>
        /// <param name="font">Nouvelle Police  (respecter les espaces et la casse)</param>
        /// <param name="size">Taille désirée</param>
        [MessageCallback (Key ="ChangerPoliceEtTaille")]
        void changeFontAndFontSize(string font, int size)
        {
            try
            {
                label1.Font = new Font(font, size);
                PackageHost.PushStateObject("PoliceDeCaractere", label1.Font.FontFamily.ToString());
                PackageHost.PushStateObject("TaillePolice", label1.Font.Size);

                label1.Size = new System.Drawing.Size(this.Height, this.Width);
            }
            catch(Exception e)
            {
                PackageHost.WriteInfo("Orthographe de la police ou taille de caractère incorrecte");
            }
        }


    }
}
