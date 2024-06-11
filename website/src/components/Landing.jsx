import React from "react";
import "../styles/landing.css";
import { PlayCircle } from "react-bootstrap-icons";

const Landing = ({ handleOnPlayClick }) => {
  return (
    <>
      <div className="row">
        <div className="column">
          <h2 className="black-title">WCD</h2>
          <h2 className="color-title">BURGER</h2>
        </div>
      </div>
      <div className="row">
        <img className="image" src="burger.png" alt="burger image" />
      </div>
      <div className="row">
        <button className="play-button" onClick={handleOnPlayClick}>
          <span className="icon-text-button">
            <PlayCircle />
            &nbsp;&nbsp;Play Now!
          </span>
        </button>
      </div>
    </>
  );
};

export default Landing;
