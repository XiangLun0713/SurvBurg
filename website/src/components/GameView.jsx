import React, { useCallback, useEffect, useState } from "react";
import { useUnityContext, Unity } from "react-unity-webgl";
import { ArrowsFullscreen } from "react-bootstrap-icons";
import { updatePlayerScoreInDatabase } from "../services/updateLeaderboard";
import { getLeaderboardFromDatabase } from "../services/fetchLeaderboard";
import "../styles/gameview.css";

const GameView = () => {
  const [leaderboard, setLeaderboard] = useState("");
  const username = localStorage.getItem("username");

  const {
    isLoaded,
    unityProvider,
    requestFullscreen,
    addEventListener,
    removeEventListener,
    sendMessage,
  } = useUnityContext({
    loaderUrl: "survburg/survburg.loader.js",
    dataUrl: "survburg/survburg.data",
    frameworkUrl: "survburg/survburg.framework.js",
    codeUrl: "survburg/survburg.wasm",
  });

  const onFullScreen = () => {
    requestFullscreen(true);
  };

  const handleGetLeaderboard = useCallback(() => {
    getLeaderboardFromDatabase(username).then((result) =>
      setLeaderboard(result)
    );
  }, []);

  const handleUpdateUserHighScore = useCallback((highscore) => {
    updatePlayerScoreInDatabase(username, highscore);
  }, []);

  useEffect(() => {
    addEventListener("GetLeaderboard", handleGetLeaderboard);
    addEventListener("UpdateUserHighScore", handleUpdateUserHighScore);
    return () => {
      removeEventListener("GetLeaderboard", handleGetLeaderboard);
      removeEventListener("UpdateUserHighScore", handleUpdateUserHighScore);
    };
  }, [
    addEventListener,
    removeEventListener,
    handleUpdateUserHighScore,
    handleGetLeaderboard,
  ]);

  useEffect(() => {
    sendMessage(
      "Main Menu Manager",
      "UpdateLeaderboardContext",
      JSON.stringify(leaderboard)
    );
    return () => {};
  }, [leaderboard]);

  return (
    <div className="game-container">
      <div className="row relative">
        <div
          id="loader"
          style={{ visibility: isLoaded ? "hidden" : "visible" }}
        >
          <div id="box"></div>
          <div id="hill"></div>
        </div>
        <Unity unityProvider={unityProvider} className="game-view" />
      </div>
      <div
        className="button-row"
        style={{ visibility: isLoaded ? "visible" : "hidden" }}
      >
        <button className="fullscreen-button" onClick={onFullScreen}>
          <span className="icon-text-button">
            <ArrowsFullscreen />
            &nbsp;&nbsp;Fullscreen
          </span>
        </button>
      </div>
    </div>
  );
};

export default GameView;
