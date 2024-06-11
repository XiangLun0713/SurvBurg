import { firestore } from "./firebase";
import { getDoc, doc, setDoc, updateDoc } from "firebase/firestore";

const updatePlayerScoreInDatabase = async (username, highscore) => {
  const currentPlayerDocRef = doc(firestore, "player", username);
  const documentSnapshot = await getDoc(currentPlayerDocRef);

  if (documentSnapshot.exists()) {
    // Compare the value and update it if the score in db is lesser
    const scoreFromDatabase = documentSnapshot.data().score;
    if (highscore > scoreFromDatabase) {
      await updateDoc(currentPlayerDocRef, { score: Number(highscore) });
    }
  } else {
    // Create a document with username from local storage, and instantiate the username and score fields
    await setDoc(currentPlayerDocRef, {
      score: highscore,
      name: username,
    });
  }
};

export { updatePlayerScoreInDatabase };
