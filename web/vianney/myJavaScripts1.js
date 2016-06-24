var recipe_choice = {
    "m_conditions": [
      {
          "ArgumentType": "boolean",
          "variables": {
              "sentinelle": "DESKTOP-FQMIBUN",
              "package": "ConstellationPackageConsole1",
              "variable": "myValue1"
          }
      },
      {
          "ArgumentType": "boolean",
          "variables": {
              "sentinelle": "DESKTOP-FQMIBUN",
              "package": "ConstellationPackageConsole1",
              "variable": "myValue2"
          }
      },
      {
          "ArgumentType": "boolean",
          "variables": {
              "sentinelle": "DESKTOP-FQMIBUN",
              "package": "ConstellationPackageConsole1",
              "variable": "myValue3"
          }
      }
    ],
    "m_realisations": [
      {
          "ArgumentType": "boolean",
          "variables": {
              "sentinelle": "DESKTOP-FQMIBUN",
              "package": "ConstellationPackageConsole1",
              "callBack": "changeVal1"
          }
      },
       {
           "ArgumentType": "boolean",
           "variables": {
               "sentinelle": "DESKTOP-FQMIBUN",
               "package": "ConstellationPackageConsole1",
               "callBack": "changeVal2"
           }
       },
        {
            "ArgumentType": "boolean",
            "variables": {
                "sentinelle": "DESKTOP-FQMIBUN",
                "package": "ConstellationPackageConsole1",
                "callBack": "changeVal3"
            }
        }
    ],
    "estRestreintHorairement": false,
    "Name": "1er algo"
};

var Condition_Array =[];
function getCondition(number) {
    var Condition = {};
    Condition["ArgumentType"] = recipe_choice.m_conditions[number].ArgumentType;
    Condition["sentinelle"] = recipe_choice.m_conditions[number].variables.sentinelle;
    Condition["package"] = recipe_choice.m_conditions[number].variables.package;
    Condition["variable"] = recipe_choice.m_conditions[number].variables.variable;//name use

    Condition_Array.push(Condition);
}

var Realisations_Array =[];
function getRealisations(number) {
    var Realisations = {};
    Realisations["ArgumentType"] = recipe_choice.m_realisations[number].ArgumentType;
    Realisations["sentinelle"] = recipe_choice.m_realisations[number].variables.sentinelle;
    Realisations["package"] = recipe_choice.m_realisations[number].variables.package;
    Realisations["callBack"] = recipe_choice.m_realisations[number].variables.callBack;//name use

    Realisations_Array.push(Realisations);
}

//Global var
var Condition_Available = [];
var Realistion_Available = [];

var Condition_Taken = [];
var Realisation_Taken = [];
Condition_Taken["name"] = [];
Realisation_Taken["name"] = [];

Condition_Taken["libelle"] = [];
Realisation_Taken["libelle"] = [];

Condition_Taken["boolean"] = [];
Realisation_Taken["boolean"] = [];


Condition_Taken["ChangeSaved"] = [];
Realisation_Taken["ChangeSaved"] = [];



//starter function
function getData() {
    var i = 0;
    while (recipe_choice.m_conditions[i] !=undefined) {
        getCondition(i);

        Condition_Available.push(Condition_Array[i].variable);
        i++;
    }
    i = 0;
    while (recipe_choice.m_realisations[i] != undefined) {
        getRealisations(i);

        Realistion_Available.push(Realisations_Array[i].callBack);
        i++;
    }
    Condition_Available.sort();
    Realistion_Available.sort();
}

function update(Type) {
    //IF
    var AllLigne = "";
    if (Type === "If") {

        blank = "blankIf";
        var blank = document.getElementById(blank);
        for (var i = 0; i < Condition_Taken["name"].length; i++) {
            AllLigne = AllLigne + createLigne(Type, i);
        }
    }
    //THEN
    else {
        blank = "blankThen";
        var blank = document.getElementById(blank);
        for (var i = 0; i < Realisation_Taken["name"].length; i++) {
            AllLigne = AllLigne + createLigne(Type, i);
        }
    }
    blank.innerHTML = "<table class='table table-striped'>"
					+ AllLigne
		            + "</table>";
}

function addElement(Type) {
    //IF
    if (Type === "If") {
        Condition_Taken["name"].push("Undefined");
    }
    //THEN
    else {
        Realisation_Taken["name"].push("Undefined");
    }
    update(Type);
}


function createLigne(type,indice) {//i=indice
    var dropList;
    var name ="";
    if (type === "If") {
        dropList = Condition_Available;
        name = Condition_Taken["name"][indice];
    }
    else {
        dropList = Realistion_Available;
        name = Realisation_Taken["name"][indice];
    }
    //add libelle in the name 
    var type_Taken = getTypeTaken(type);
    if (type_Taken["libelle"][indice] != null) {
        name = name + type_Taken["libelle"][indice];
    }

    //droplist choice
    var conditionChoice = "";
    for (var j in dropList) {
        conditionChoice = conditionChoice + "<li><a href='#' onclick ='selectElement(" + "\"" + type + "\"," + indice + "," + "\"" + dropList[j] + "\"" + ") '>" + dropList[j] + "</a></li>";
       
    }
    //modale window
    var modalWindow = "";
    if (name != "Undefined") {
        modalWindow = createModalWindow(type, indice);
    }

    //create one ligne
    newLigne = "<tbody>"
	+ "<tr>"
    + "<td>" + name + "</td>"
    + "<td>"
        + "<div class='btn-group pull-right'>"

            + "<button class='btn btn-info' data-toggle='modal' data-target='#myModal" + type + indice + "'>"
            	+ "<span class='glyphicon glyphicon-cog'></span>"
            + "</button>"

            + modalWindow

            + "<button class='btn btn-primary dropdown-toggle' href='#' data-toggle='dropdown'>"
            	+ "<span class='glyphicon glyphicon-plus'></span>"
            	+ "<span class='caret'></span>"
            + "</button>"
            + "<ul class='dropdown-menu stay-open pull-right' role='menu' style='padding: 15px; min-width: 300px;'>"
            	+ conditionChoice
            + "</ul>"
           	+ "<button class='btn btn-danger' onclick ='deleteElement(" + "\"" + type + "\"," + indice + ")' >"
            	+ "<span class='glyphicon glyphicon-remove'></span>"
            + "</button>"
        + "</div>"
   + "</td>"
   + "</tr>"
   + "</tbody>";
    return newLigne;
}


function selectElement(type, indice, elementSelected) {
    if (type === "If") {
        Condition_Taken["name"][indice] = elementSelected;
        update("If");
    }
    else {
        Realisation_Taken["name"][indice] = elementSelected;
        update("Then");
    }
}

function deleteElement(type, indice) {
    if (type === "If") {
        Condition_Taken["name"].splice(indice, 1);//remove
        update("If");
    }
    else {
        Realisation_Taken["name"].splice(indice, 1);//remove
        update("Then");
    }
}

function createModalWindow(type, indice) {
    //get the name
    var modalWindow = "";
    var type_Taken;
    var indice_type_array = -1;
    var name = getName(type, indice);

    var typeOfVar = "unknown";
    //get the Argument from the name of the element selected
    //If
    if (type === "If") {
        for (var j in Condition_Array) {
            if (Condition_Array[j]["variable"] === name) {
                indice_type_array = j;
            }
        }
        type_Taken = Condition_Taken["name"];
        typeOfVar = Condition_Array[indice_type_array]["ArgumentType"];
    }
    //Else
    else {
        for (var j in Realisations_Array) {
            if (Realisations_Array[j]["callBack"] === name) {
                indice_type_array = j;
            }
        }
        type_Taken = Realisation_Taken["name"];
        typeOfVar = Realisations_Array[indice_type_array]["ArgumentType"];
    }
    if ((typeOfVar != "int")||(typeOfVar != "boolean")) {
        //create the modal window
        modalWindow = "<div class='modal fade' id='myModal" + type +indice+"' tabindex='-1' role='dialog' aria-labelledby='myModalLabel' aria-hidden='true'>"
            + "<div class='modal-dialog' role='document'>"
            + "<div class='modal-content'>"
                + "<div class='modal-header'>"
                + "<button type='button' class='close' data-dismiss='modal' aria-label='Close'>"
                    + "<span aria-hidden='true'>&times;</span>"
                + "</button>"
                + " <h4 class='modal-title' id='myModalLabel'>" + name + "</h4>"
                + "</div>"
                + "<div class='modal-body'>";
        //===INTEGER===//
        if (typeOfVar === "int") {
           
        }
        //===BOOLEAN===//
        if (typeOfVar === "boolean") {
            var idA = "\"" + type + "\"" + "," + indice + "," + "\"" + "buttonTrue" + "\"";
            var idB = "\"" + type + "\"" + "," + indice + "," + "\"" + "buttonFalse" + "\"";
            var idC = "\"" + type + "\"" + "," + indice + "," + "\"" + typeOfVar + "\"";
            modalWindow = modalWindow + "<div class='btn-group' id='toggle_event_editing'>"
	            + "<button type='button' id ='" + type + indice + 'buttonTrue' +"' class='btn btn-info locked_active'        onclick ='buttonBoolean("+idA+")'>ISTRUE</button>"
	            + "<button type='button' id ='" + type + indice + 'buttonFalse'+"' class='btn btn-default unlocked_inactive' onclick ='buttonBoolean("+idB+")'>ISFALSE</button>"
            + "</div>"
        }
        modalWindow = modalWindow + "</div>"
                + "<div class='modal-footer'>"
                + "<button type='button' class='btn btn-secondary' data-dismiss='modal'>Close</button>"
                + "<button type='button' class='btn btn-primary' onclick ='saveChangeModalWindow(" + idC + ")'>Save changes</button>"
                + "</div>"
            + "</div>"
            + "</div>"
        + "</div>";
        //value by Default
        var type_Taken = getTypeTaken(type);
        console.log(type_Taken["ChangeSaved"][indice]);
        if (type_Taken["ChangeSaved"][indice] === undefined) {
            type_Taken["boolean"][indice] = true;
        }
        
    }
    return modalWindow;
}

//Button Modal window management
function buttonBoolean(type, indice, typeButton) {
    var buttonFalse = document.getElementById(type + indice + "buttonFalse");
    var buttonTrue = document.getElementById(type + indice + "buttonTrue");
    var type_Taken = getTypeTaken(type);

    if (typeButton === "buttonFalse") {
        buttonTrue.className = "btn btn-default unlocked_inactive";
        buttonFalse.className = "btn btn-info locked_active";
        type_Taken["boolean"][indice] = false;
    }
    if (typeButton === "buttonTrue") {
        buttonTrue.className = "btn btn-info locked_active";
        buttonFalse.className = "btn btn-default unlocked_inactive";
        type_Taken["boolean"][indice] = true;
    }
    type_Taken["ChangeSaved"][indice] = false;
}

function saveChangeModalWindow(type, indice, typeOfVar) {
    var type_Taken = getTypeTaken(type);
    type_Taken["ChangeSaved"][indice] = true;
    //write the libelle next to the name

    //int

    //boolean
    if (typeOfVar === "boolean") {
        if (type_Taken["boolean"][indice] === true) {
            type_Taken["libelle"][indice] = "Is True"
        }
        else {
            type_Taken["libelle"][indice] = "Is False"
        }
    }
    update();
}


//Utility
function updateModalWindow(type, indice) {
    if (getName(type, indice) != "Undefined") {
        var modalSpace = document.getElementById(type + indice + 'modalWindow');
        modalSpace.innerHTML = createModalWindow(type, indice);
    }
}
function closeUpdateWindow(type, indice) {
    if (getName(type, indice) != "Undefined") {
        var modalSpace = document.getElementById(type + indice + 'modalWindow');
        modalSpace.innerHTML = "";
    }
}
function getTypeTaken(type) {
    var type_Taken;
    if (type === "If") {
        type_Taken = Condition_Taken;
    }
    else {
        type_Taken = Realisation_Taken;
    }
    return type_Taken;
}
function getName(type, indice) {
    if (type === "If") {
        dropList = Condition_Available;
        name = Condition_Taken["name"][indice];
    }
    else {
        dropList = Realistion_Available;
        name = Realisation_Taken["name"][indice];
    }
    return name;
}


//SAVE
var recipe_chose = [];
function save() {
    recipe_chose = [];
    var m_conditions = [];
    var m_realisations = [];
    recipe_chose.push(m_conditions);
    recipe_chose.push(m_realisations);
    //put condition
    for (var i in Condition_Array) {
        if (Condition_Taken["name"].indexOf(Condition_Array[i].variable) >= 0) {
            m_conditions.push(Condition_Array[i]);
        }
    }
    //put realisation
    for (var i in Realisations_Array) {
        if (Condition_Taken["name"].indexOf(Realisations_Array[i].variable) >= 0) {
            m_realisations.push(Realisations_Array[i]);
        }
    }
}