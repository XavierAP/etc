# crypto
Homework in C# from the Stanford "massive open online course" [crypto-class.org](http://crypto-class.org)
which I did not finish -- extremely cool but extremely outside my field of professional interest.

- [**`BirthdayAttack.cs`**](BirthdayAttack.cs)
Apparently a [birthday attack](https://en.wikipedia.org/wiki/Birthday_attack)
(finding two messages whose hashes collide).
At the time I didn't deign to write any comment in this code.
Apparently the program throws on success and returns silently on failure?
Nice.
- [**`ManyTimesPad.cs`**](ManyTimesPad.cs)
An attack on an [one-time pad](https://en.wikipedia.org/wiki/One-time_pad)
that has been improperly used many times for different messages (many times pad)
creating the vulnerability we exploit.
The program isn't fully automatic: a first part outputs some partially decrypted
(probably, relying on bytes having ASCII meaning)
which a human has inspected (I have) to guess the plain text message,
with which the rest of the program obtains the key
and decrypts all messages encrypted with the same one.
- [**`WeakPRG.cs`**](WeakPRG.cs)
Apparently an attack on a weak [pseudo-random generator](https://en.wikipedia.org/wiki/Pseudorandom_generator),
guessing the next number to be generated.
Again I wrote no comments into this short code
(with short course deadlines).
- [**`utils/Hex.cs`**](utils/Hex.cs)
Just two functions to transcode (encrypted) messages between byte arrays and string representations in hexadecimal
-- as they were provided in the course.
Some of the programs above depend on this.
