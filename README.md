# ISN Chess Game

## Organisation des coordonnées des pions *(BACKEND)*

### Nom et organisation des **PIONS**
Camps: 
- Black : `b` 
- White : `w`

I.D. Rôles:
- Dame: `1`
- Roi: `2`
- Fou: `3`
- Tour: `4`
- Pion: `5`
- Cavalier: `6`

### Nom et organisation des **BOARD**
Le Board est une `Matrice de 8*8` où l'origine est dans le coin en haut à gauche

Les indices sont de forme `board[y][x]` où `x` et `y` étant les coordonnées de chaques cases. Chaque élément retourné de `board[y][x]` est une `liste` vide si la case et vide, de la forme `[rôle, camp, numéro]` si une pièce est sur la case...



		
