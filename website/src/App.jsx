import React, { useState, useEffect } from "react";
import Landing from "./components/Landing";
import "./styles/app.css";
import Modal from "./components/Modal";
import GameView from "./components/GameView";

const App = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [displayGame, setDisplayGame] = useState(
    localStorage.getItem("username") !== null
  );

  const handleOnPlayClick = () => {
    if (localStorage.getItem("username") === null) {
      setIsOpen(true);
    } else {
      setDisplayGame(true);
    }
  };

  // const handleMouseMove = (e) => {
  //   document.querySelectorAll(".img-component").forEach((component) => {
  //     const speed = component.getAttribute("data-speed");

  //     const x = (window.innerWidth - e.pageX * speed) / 100;
  //     const y = (window.innerHeight - e.pageY * speed) / 100;

  //     component.style.transform = `translateX(${x}px) translateY(${y}px)`;
  //   });
  // };

  // useEffect(() => {
  //   window.addEventListener("mousemove", handleMouseMove);

  //   return () => {
  //     window.removeEventListener("mousemove", handleMouseMove);
  //   };
  // }, []);

  return (
    <div>
      <Modal
        isOpen={isOpen}
        setIsOpen={setIsOpen}
        setDisplayGame={setDisplayGame}
      />
      <div className="content-container">
        {displayGame ? (
          <GameView />
        ) : (
          <Landing handleOnPlayClick={handleOnPlayClick} />
        )}
      </div>
    </div>
  );
};

export default App;
