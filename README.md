# wi_game_techpriests

Projekt gry promującej Wydział Informatki AGH tworzony przez zespół *TechKapłani* w ramach przedmiotu *Inżynieria Oprogramowania*.

## Definition of Done:

- Funkcjonalność działa w środowisku lokalnym
- Przechodzą wszystkie stworzone testy jednostkowe
- Co najmniej 1 ososba zrobiła code review do pr
- Zmiany skutecznie mergują się do main
- (opcjonalnie) przechodzi workflow automatycznej kompilacji do WebGL i deploymentu na github pages

## Funkcjonalności:

#### Rozgrywka

* Rozgrywka każdej z minigier działa z osobna
* Gracz dołącza do rozgrywki po podaniu kodu sesji i unikalnego nicku
* Można przechodzić pomiędzy kolejnymi etapami rozgrywki (w jednym kierunku))
* Jest możliwe zagranie ponownie w daną minigrę (max 3 razy)
* Po każdej rozgrywce w minigrę otrzymujemy informację o wyniku
* Pod koniec rozgrywki otrzymujemy informację o całkowitym wyniku
* Możemy zobaczyć nasz wynik w porównaniu z innymi graczami (z podziałem na poszczegolne minigry + całkowity) na leaderboardzie

#### Backend

1. Można tworzyć wiele sesji w tym samym czasie.
2. Gracz nie może się zapisać do nieaktywnej/nieistniejącej sesji
3. Gracz nie może mieć nicku już istniejącego w ramach danej sesji
4. Tylko zapisani gracze mają dostęp do pobierania danych minigier i publikowania wyniku (token)
5. Gracz może podejrzeć wyniki graczy wyłącznie z sesji do której jest zapisany
6. Dane do minigier są wybierane losowo z bazy danych
