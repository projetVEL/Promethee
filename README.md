# ![alt tag](https://github.com/projetVEL/projetVEL/blob/master/logo.png) Prométhée
<i>Par Vianney GUISON, Edouard FRANCOIS et Louis-Jean CLAEYSSEN</i>
<br/>
<p>
Bienvenue sur la page de Prométhée (anciennement projetVEL), un projet pour <a href="http://www.myconstellation.io/">Constellation</a> pensé pour vous simplifier la vie. 
<br/>
Nous nous inspirons de l'application IF de IFTTT sur Android pour créer une interface utilisateur simple, permettant à un néophyte n'ayant jamais codé de pouvoir faire des conditions et actions simples dans un environnement Constellation. Ainsi seul le nom de la sentinelle, du package et de la variable à modifier sont à savoir, il ne lui reste plus qu'à déterminer la valeure de la variable à observer/changer pour créer des interactions.
<br/>
Ex : si le StateObject "IsOn" d'un bouton sur le pc1 est "True" alors on appel le MessageCallBack "DisplaySomething('bouton ON')" sur le pc2.
</p>
<br />
<p>
Pour l'installation sur votre Constellation :<br/>
<ul>
<li>Déployez le package AlgorithmePackage</li>
<li>Changez les chemins web dans les fichiers HTML et JS (sauf si vous hébergez le site sur vote localHost : cf wamp)</li>
<li>Changez les clefs de connexion Constellation</li>
<li>Déployez le site web (nous vous conseillons <a href="http://www.wampserver.com/">wamp</a> pour un usage local sous windows)</li>
<li>Déployez vos packages à tester (nous vous conseillons <a href="https://github.com/projetVEL/projetVEL/tree/master/Packages%20Annexes">nos packages de test</a> si vous n'avez aucun package à essayer)</li>
<li>Enjoy !</li>
</ul>
</p>
<div style="text-aligne:center"> -------------------------------------------</div>
<br/>
Fonctionnement détaillé des packages (FrontEnd Web et BackEnd C#) <br/>
<b> /!\ Documentation technique nécessitant des connaissances de base en programmation et du fonctionnement de Constellation. Cette partie est reservée à un public averti voulant comprendre le fonctionnement interne des Packages de Prométhée /!\ </b>
<br/><br/>
Note : Ici les règles sont appelées "Algorithmes" et contiennent une liste de conditions qui si vérifiées appel les MessageCallback d'une liste d'exécutions.
<br/><br/>
Package <b>Web</b> :
<p>
Lorsque la page princiaple "index.html" se lance, le script AngularJS va se connecter au server Constellation et faire une requête de StateObjects de toutes les Sentinelles sur lesquelles est installé le BackEnd C# ("AlgorithmePackage"), et choisit la première Sentinelle qui a les deux listes de règles (celles en pauses et celles actives) et ne prend la Sentinelle "Developer" que s'il n'y a pas d'autre Sentinelle ayant le BackEnd déployé. <br/>
Le script a alors choisit la sentinelle cible et affiche tous les "Algorithmes" récupérés par les StateObjects.
</p>
<p>
Lors du clique sur "Voir un Algorithme" on charge la page "algorithme.html" en donnant en paramètre GET, la Sentinelle sur laquelle aller chercher la liste des algorithme et le nom de l'Algorithme à afficher. Le script charge alors un algorithme type vide (sans nom, sans description, ...) puis change les informations avec celles de l'Algorithme à charger (la selectionne dans les listes de règles en utilisant le nom reçu par GET comme clef), si le nom ne correspond à aucun élément dans la liste (ou s'il n'est pas donné comme lorsque l'on créer un nouvel Algorithme) le script laisse les champs par défaut, puis affiche les éléments de l'Algorithme (conditions, exécutions, plages et restriction horaire). <br/>
En parallèle, le modle Angular fait plusieurs requêtes via les APIs 'constellationConsumer' et 'constellationController' pour créer un arbre repésentant la Constellation : Sentinelles - Packages - StateObjecte/MessageCallbacks. Cet arbre est utilisé lors du choix des conditons/exécutions pour savoir quel(s) Package(s) afficher si on selectionne telle ou telle Sentinelle, ...
<br/>
Lorsque l'on clique sur "Supprimer Algorithme", le controller AngularJS appel le MessageCallBack d'"AlgorithmePackage" sur la Sentinelle désignée précédement en lui donnant en paramètre le nom de l'Algorithme à supprimer.
<br/>
Lorsque l'on "Envoit à Constellation" la règle créée, le script supprime les conditions et exécution inutiles (pas de Package, Sentinelle ou StateObject/MassageCallBack renseigné) puis l'envoit au BackEnd pas MessageCallback.
</p>
Package <b>C#</b> : 
<p>
Lorsqu'il est lancé, le Package va récupérer les anciens StateObjects (liste des Algorithmes en pause et celle des Algorithme en cours) qu'il avait envoyé à Constellation précédemment. S'il n'en avait pas envoyé précédemment ou que l'on a purgé les StateObjects ou que Constellation mets plus de 2.5secondes à répondre, le Package envoit un des StateObjects 'vides' (deux listes vides). Les listes sont mises à jour sur Constellation à chaque suppression ou ajout d'Algorithme, ou lors de la fermeture du Package.
<br/>
Si le BackEnd reçoit un MessageCallback pour supprimer une règle, il parcourt les deux listes d'Algorithmes et supprime celle correspondant au nom donné en paramètre.
<br/>
Lorsque le MessageCallback du Package pour ajouter une règle est appelé, il vérifie que l'Algorithme a bien un nom (sinon lui en associe un en 'hashant' l'objet), qu'il a une description (sinon lui en donne un en appelant sa méthode toString()) et qu'il a bien une URL de description pour la photo (sinon lui en associe une par défaut). Le programme vérifie ensuite qu'un Algorithme de même nom n'existe pas déjà et si oui, l'écrase par le nouveau. Puis vérifie si la règle est active et si elle doit ou non être mise en pause le temps de rentrer dans sa plage horaire (ex : si une règle ne peut s'éxecuter que pendant le week-end et qu'elle est ajouté Vendredi, elle sera mise en pause jusque Samedi minuit). Enfin, si l'algorithme est actif, le package s'abonne aux StateOnjects des conditions à surveiller.
<br/>
Quand un StateObject est reçu, le BackEnd parcourt la liste des Algorithmes en cours et détermine s'ils sont encore dans leur plage horaire (sinon les désactive jusqu'à leur prochaine plage), détermine si le StateObject reçu est surveillé par cette règle et si oui, vérifie que toutes les conditions sont remplis pour pouvoir l'exécuter.
<br/>
Lors de l'exécution d'une règle, le programme parcours ses conditions et appel les MessageCallBacks correspondant. Si l'Algorithme est restreint horairement (ne peut s'exécuter qu'une fois par minute, heure, ... ou toutes les X secondes), alors il est mis en pause.
<br/>
Si un Algorithme est mis en pause alors il est retiré de la liste des Algorithmes actifs et ajouté dans celle des règles en pauses. Une entrée est aussi ajoutée dans la table des Algorithmes à réactiver : on associe le nom d'un Algorithme en pause au temps en seconde qu'il doit attendre avant sa réactivation.
<br/>
En parallèle, de la vérification des règles en cours, est lancé un Thread surveillant les Algorithmes en pause et décomptant le temps restant avant leur réactivation. Si une règle doit être réactivée, alors le Package se réabonne aux StateObjects des conditions de cette dernière et l'Algorithme est placé dans la liste active. Ce Thread sert aussi lorsqu'aucun évênement d'update de StateObject n'est reçu pendant une demi secondes ; ce thread appel alors la méthode qui vérifie que toutes les conditions des Algorithmes sont bonnes.
</p>
Pour plus d'informations, nous vous invitons à parcourir le code des différents packages.
