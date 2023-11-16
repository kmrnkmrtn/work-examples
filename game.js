const menuDiv = document.querySelector('#menu')
const gameDiv = document.querySelector('#game')

const table = document.querySelector("#table")
const p1cats = document.querySelector("#p1cats")
const p2cats = document.querySelector("#p2cats")
const screen = document.querySelector("#screen")
const saveBtn = document.querySelector("#save")
const scoreboardBtn = document.querySelector("#score")
const scoreBoard = document.querySelector("#scoreboard")
const Board = document.querySelector('#board')

//menü
const name1Text = document.querySelector('#name1')
const name2Text = document.querySelector('#name2')
const tabsizeText = document.querySelector('#tabsize')
const catsizeText = document.querySelector('#catsize')
const maxscoreText = document.querySelector('#maxscore')
let names = ["a","b"]
let meows = []

 meows[0] = new Audio('sounds/meow.mp3');
 meows[1] = new Audio('sounds/meow2.mp3');
 meows[2] = new Audio('sounds/meow3.mp3');
 meows[3] = new Audio('sounds/meow4.mp3');
 meows[4] = new Audio('sounds/meow5.mp3');

function rand(){
   return Math.floor(Math.random()*4)
} 


document.querySelector('#start').addEventListener("click", startGame)
if(localStorage.getItem("SB") !== null) { Board.innerHTML = localStorage.getItem("SB") }



let catTables= [0,0]
let gameTable = [] 
let gameTableTEMP = []
let currentPlayer = 0
let CAT_NUM = 8
let TABLE_SIZE = 6
let gameOver = true
let playerPoints = [0,0]
let MAX_SCORE = 5
let winner




function startGame(){
    if(!tryReadInput())
    return
    gameTable = []
    menuDiv.hidden = true
    gameDiv.hidden = false
    gameOver = false;
    saveBtn.value = "Save"
    createGameTable(gameTable,TABLE_SIZE)
    refreshTable()
    createCats(CAT_NUM)
    refreshCatTables()
}

function newGame(){
    table.innerHTML = "";
    p1cats.innerHTML  = "";
    p2cats.innerHTML = "";
    screen.innerHTML = "Player 2 starts!";
    currentPlayer = 0
    gameTable = []
    playerPoints = [0,0]
    catTables = [0,0]
    gameOver = false;
    createGameTable(gameTable,TABLE_SIZE)
    refreshTable()
    createCats(CAT_NUM)
    refreshCatTables()

}


function tryReadInput() {
    if (name1Text.value === "" 
        || name2Text.value === ""
        || tabsizeText.value === ""
        || catsizeText.value === ""
        || maxscoreText.value === ""
        || maxscoreText.value < 1
        || maxscoreText.value > 15
        || tabsizeText.value < 5
        || tabsizeText.value > 12 
        || catsizeText.value > 12
        || catsizeText.value < 4 ) 
         
        return false;

    TABLE_SIZE  = parseInt(document.querySelector('#tabsize').value)
    CAT_NUM = parseInt(document.querySelector('#catsize').value)
    MAX_SCORE = parseInt(document.querySelector('#maxscore').value)
    names[0] = name1Text.value
    names[1] = name2Text.value
    
    return true
}

document.addEventListener('keydown', e => {
    if(e.key === 'r') { 
        newGame()
       
      } 
});


saveBtn.addEventListener("click", e => {
    if(gameDiv.hidden == true) { load() }
    if(gameDiv.hidden == false) { save() }
})

var board = false
scoreboardBtn.addEventListener("click", e => {
    if(board === true && gameOver === false) { gameDiv.hidden = false; scoreBoard.hidden = true; board = false; return;}
    if(board === true && gameOver === true ) { menuDiv.hidden = false; scoreBoard.hidden = true; board = false; return;}
    

    
    board = true;
    menuDiv.hidden = true;
    gameDiv.hidden = true;
    scoreBoard.hidden = false;


})


var players = []
var rows = 0
function addScore(){
    if(gameOver === false) { return }
    players.push(names[1])
    players.push(names[0])
    players.push(winner)
    console.log(players)
    var d = new Date();
    Board.innerHTML += `<tr> <td> ${players[1+(rows*3)]}  </td>  <td>  ${players[0+(rows*3)]}</td>
     <td> ${names[players[2+(rows*3)]]}  </td> <td>${d.toLocaleString()}</td> </tr>` 
    rows++
    localStorage.setItem("SB", Board.innerHTML);


}

table.addEventListener("click", e => {
    console.log(e.target)
    if(!(e.target.matches("td")||e.target.matches("td img"))) return;
    const x = e.target.closest("td").cellIndex
    const y = e.target.closest("tr").rowIndex
    step(x,y);
} )

var timeoutId = null;
table.addEventListener("mouseover", e => {
    
    if(!(e.target.matches("td")||e.target.matches("td img"))) return;
    timeoutId = window.setTimeout(function(){
        
        const x = e.target.closest("td").cellIndex
        const y = e.target.closest("tr").rowIndex
        console.log(x+" "+y)
        //show(x,y)
        
        
        
    }, 2000);
    
    
    
})

table.addEventListener('mouseout',function() { 
    window.clearTimeout(timeoutId)
    
  });

function createCats(size){
    CAT_NUM = size
    for(let i=0; i<size;i++){
        
        catTables[0]++
        catTables[1]++
    }
}



function createGameTable(table,size) {
    TABLE_SIZE = size
    for(let i=0; i< size; i++){
        let row = []
        for(let j=0; j< size; j++){
            row.push(2)
        }  
        table.push(row)      
    }
}



function refreshCatTables(){
    
    let str0 = ""
    let str1 = ""
    for(let i = 0; i<CAT_NUM; i++)
    {
       
        if(i<catTables[0]){ catimg = "cat0.jpg"; }else{ catimg = "nocat.jpg" }
        str0 += `<td> <img src="${catimg}" width="50" height="50"  ></td>`
        if(i<catTables[1]){ catimg = "cat1.jpg"; }else{ catimg = "nocat.jpg" }
        str1 += `<td> <img src="${catimg}" width="50" height="50"  ></td>`
    }
    str0 += "</tr>"
    str1 += "</tr>"
    p1cats.innerHTML = str0;
    p2cats.innerHTML = str1;
}




function refreshTable() {

    let str = ""
    gameTable.forEach( row => {
        str += "<tr>"
        row.forEach( cell => {
            //let color = cell === 2 ? "white" :  cell === 1 ? "blue" : "red"
            let catimg = cell === 2 ? "nocat.jpg" :  cell === 1 ? "cat1.jpg" : "cat0.jpg"
            
            
            //str += `<td style="background-color: ${color}"> ${cell} </td>`
            str += `<td> <img src="${catimg}" width="100" height="100" ></td>`

          

        } )
        str += "</tr>"

    } )
    
    table.innerHTML = str
 
}

function step(x, y){
    if(gameOver) return;
    if(gameTable[y][x] !== 2) return
    meows[rand()].play();
    // game over by all cats placed
    if(catTables[0] === 0 || catTables[1] === 0){ 
         winner = catTables[0] === 0 ? 1 : 0; 
         
         console.log(winner+" wins"); 
         gameOver = true; 
         addScore();
         
         screen.innerHTML = "game over by all cats placed "+names[winner]+" wins"; return;
         
        }
    //console.log(x+" "+y)
    pushAll(x,y)
    if((currentPlayer === 1 && catTables[1] > 0)||(currentPlayer === 0 && catTables[0] > 0)) { 
         gameTable[y][x] = currentPlayer
         currentPlayer === 1 ? catTables[1]-- : catTables[0]--
        
    }
    
    currentPlayer = ((currentPlayer + 1) % 2)
    checkPoints()
    if(gameOver) {screen.innerHTML = "game over by gathering "+MAX_SCORE+" points"; return;} //add to board
    screen.innerHTML="current player: "+names[currentPlayer]+"🐱🐱🐱"+names[1]+" score: "+playerPoints[1]+"🐱🐱🐱"+names[0]+" score: "+playerPoints[0]
    refreshTable()
    refreshCatTables()
}


function pushAll(x,y){
    if(y===TABLE_SIZE-1||x===TABLE_SIZE-1||y===0||x===0) { pushAllFrame(x,y); return; }
    //"belul" van vagy szelso kereten
    for(let i = -1; i<=1; i++){
        for(let j = -1; j<=1; j++){
            console.log("x:"+j+" y:"+i)
            push(x,y,j,i)
            
        }
    }
   
}



function push(x,y,j,i){
    
    if(gameTable[y+i][x+j] === 2) return;
    //pushdown 
    if(y+i+i>TABLE_SIZE-1 || x+j+j>TABLE_SIZE-1 || y+i+i<0 || x+j+j<0 ){
        //cat goes back to bench 
        if(gameTable[y+i][x+j] == 1 && catTables[1]<CAT_NUM){
            catTables[1]++
        }else{ if(catTables[0]<CAT_NUM) {  catTables[0]++ } }
        //cat disappear
        gameTable[y+i][x+j] = 2
        return
    }
    //push
    
    if(gameTable[y+i+i][x+j+j] === 2 ){
        gameTable[y+i+i][x+j+j] = gameTable[y+i][x+j]
        gameTable[y+i][x+j] = 2
        return
    }
}



function pushAllFrame(x,y)
{
    let i,j,ni,nj
    if(x===0) { j = 0; nj = 1; }
    if(y===0) { i = 0; ni = 1; }
    if(x===TABLE_SIZE-1) { j = -1; nj = 0; }
    if(y===TABLE_SIZE-1) { i = -1; ni = 0; }
    if(x<TABLE_SIZE-1 && x>0) { j =-1 ; nj =1 ;}
    if(y<TABLE_SIZE-1 && y>0) { i =-1 ; ni =1;}
    

    
    for(k = i; k<=ni; k++){
        for(m = j ; m<=nj; m++){
           
            push(x,y,m,k)
            
           
        }
    }

   
}
function checkPoints(){
    for(let i=0; i<TABLE_SIZE-2; i++){
        for(let j=0;j<TABLE_SIZE-2; j++){
            check3Cats(j,i,0,1)
            check3Cats(j,i,1,0)
            check3Cats(j,i,1,1)

            
        }
    }
    for(let i=0; i<TABLE_SIZE-2; i++){
        for(let j=2;j<TABLE_SIZE; j++){
            
            check3Cats(j,i,(-1),1)

            
        }
    }
    for(let i=TABLE_SIZE-2; i<TABLE_SIZE; i++){
        for(let j=0; j<TABLE_SIZE-2;j++){
            check3Cats(j,i,1,0)
            
        }
    }
    for(let i=0; i<TABLE_SIZE-2; i++){
        for(let j=TABLE_SIZE-2; j<TABLE_SIZE;j++){
            check3Cats(j,i,0,1)
            
        }
    }
 
}
function check3Cats(x,y,dx,dy){
    if(gameTable[y][x] === 2) {  return; }

    const a = gameTable[y][x]
    const b = gameTable[y+dy][x+dx]
    const c = gameTable[y+dy+dy][x+dx+dx]
    if(a === b && b === c && c === a) { 
        console.log("pont neki: "+a);
        if(playerPoints[a]<5){ 
            playerPoints[a]++;  

            if(gameTable[y][x] == 1 && catTables[1]<=CAT_NUM-3){
                catTables[1] += 3; 
            }else{ if(catTables[0]<=CAT_NUM-3) { catTables[0]+= 3;  } }

            gameTable[y][x] = 2; gameTable[y+dy][x+dx] = 2; gameTable[y+dy+dy][x+dx+dx]= 2;

             refreshTable(); refreshCatTables();
        }
        // game over by gathering 5 points
        if(playerPoints[a] === MAX_SCORE){ 
            //console.log("GAME OVER, "+a+"wins"); 
            winner = a;
            gameOver = true; 
            addScore(); 
        }
        return;
    }
    

}

function save(){
    if(gameOver === true) { screen.innerHTML = "Game is over, saving is not possible!";  return; }
    localStorage.setItem("gameTable", JSON.stringify(gameTable));
    localStorage.setItem("catTables", JSON.stringify(catTables));
    localStorage.setItem("playerPoints", JSON.stringify(playerPoints));
    localStorage.setItem("names", JSON.stringify(names));
    localStorage.setItem("CAT_NUM", CAT_NUM);
    localStorage.setItem("TABLE_SIZE", TABLE_SIZE);
    localStorage.setItem("currentPlayer", currentPlayer);
    localStorage.setItem("MAX_SCORE", MAX_SCORE);
    localStorage.setItem("screen", screen.innerHTML);
    
    
}

function load(){
    newGame();
    
    menuDiv.hidden = true
    gameDiv.hidden = false
      
    gameTable = JSON.parse(localStorage.getItem('gameTable'))
    catTables = JSON.parse(localStorage.getItem('catTables'))
    playerPoints = JSON.parse(localStorage.getItem('playerPoints'))
    names = JSON.parse(localStorage.getItem('names'))
    CAT_NUM = localStorage.getItem("CAT_NUM", CAT_NUM);
    TABLE_SIZE = localStorage.getItem("TABLE_SIZE", TABLE_SIZE);
    currentPlayer = localStorage.getItem("currentPlayer", currentPlayer);
    MAX_SCORE = localStorage.getItem("MAX_SCORE", MAX_SCORE);
    screen.innerHTML = localStorage.getItem("screen");
    gameOver = false

    refreshTable()
    refreshCatTables()  
    
    

}

//TODO: ELLOKODES HELYESEN MEGTORTENIK, PALYA KIVULI MACSKA VISSZAKERUL A MEGFELELO KISPADRA ✔️
//TODO: BARMILYEN IRANYBAN 3 CICA ESZLELESE,  -|||||- AZOK VISSZAHELYEZESE, PONTOK SZAMOLASA ✔️
//TODO: JATEK VEGE 5 PONTNAL ✔️

//TODO: KEZDOKEPERNYO + NEWGAME METHOD ALTAL JATEKINDITAS✔️
//TODO: SCOREBOARD ✔️
//TODO: PERSISTANCE (LOCAL STORAGE) ✔️
//TODO: NEWGAME BY ONE CLICK ✔️



