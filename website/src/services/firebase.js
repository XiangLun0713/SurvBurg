// Import the functions you need from the SDKs you need
import { initializeApp } from "firebase/app";
import { getAnalytics } from "firebase/analytics";
import { getFirestore } from "firebase/firestore";
import { getStorage } from "firebase/storage";
import { getAuth } from "firebase/auth";
import config from "./firebase-service.json";

const firebaseConfig = {
  apiKey: config.apiKey,
  authDomain: config.authDomain,
  projectId: config.projectId,
  storageBucket: config.storageBucket,
  messagingSenderId: config.messagingSenderId,
  appId: config.appId,
  measurementId: config.measurementId,
};

// Initialize Firebase
const firebase = initializeApp(firebaseConfig);
// const analytics = getAnalytics(app);
export const firestore = getFirestore(firebase);
export const auth = getAuth(firebase);
export const cloudStorage = getStorage(firebase);

export const postToJson = (doc) => {
  if (doc == null) return null;
  const data = doc.data();
  return {
    id: doc.id,
    ...data,
  };
};

export default firebase;
