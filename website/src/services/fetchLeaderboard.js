import { firestore } from "./firebase";
import { getDocs, collection, query, orderBy, limit } from "firebase/firestore";

const playerRef = collection(firestore, "player");

const getLeaderboardFromDatabase = async (username) => {
  const leaderboard = [];

  const q = query(playerRef, orderBy("score", "desc"), limit(10));
  const snapshot = await getDocs(q);
  snapshot.docs.forEach((doc) => {
    leaderboard.push({
      ...doc.data(),
      isCurrentUser: username === doc.data().name,
    });
  });

  return leaderboard;
};

export { getLeaderboardFromDatabase };
