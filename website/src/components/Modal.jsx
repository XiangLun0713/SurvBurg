import React, { useState, useEffect } from "react";
import ReactModal from "react-modal";
import { XLg } from "react-bootstrap-icons";
import "../styles/modal.css";

const Modal = ({ isOpen, setIsOpen, setDisplayGame }) => {
  const [showError, setShowError] = useState(false);
  const [username, setUsername] = useState("");

  const customStyles = {
    content: {
      top: "50%",
      left: "50%",
      right: "auto",
      padding: "0",
      bottom: "auto",
      transform: "translate(-50%, -50%)",
    },
  };

  useEffect(() => {
    window.addEventListener("storage", handleLocalStorageChange);

    return () => {
      window.removeEventListener("storage", handleLocalStorageChange);
    };
  }, []);

  const handleLocalStorageChange = () => {
    if (localStorage.getItem("username") === null) {
      window.location.reload();
    }
  };

  const closeModal = () => {
    setIsOpen(false);
  };

  const handleOnUsernameSubmit = () => {
    setShowError(false);
    if (username.length == 0) {
      setShowError(true);
      return;
    }
    localStorage.setItem("username", username);
    closeModal();
    setDisplayGame(true);
  };

  ReactModal.setAppElement("#root");

  return (
    <ReactModal
      isOpen={isOpen}
      onRequestClose={closeModal}
      style={customStyles}
      portalClassName="modal-style"
      contentLabel="Modal for requesting username"
      shouldFocusAfterRender={true}
      shouldCloseOnEsc={true}
      shouldCloseOnOverlayClick={true}
      preventScroll={true}
    >
      <div className="cancel-button" onClick={closeModal}>
        <XLg />
      </div>
      <div className="modal-container">
        <h4 className="title">Welcome!</h4>
        <p>
          Please enter your username. It will be displayed on the leaderboard if
          you are the top 10 player!
        </p>
        <input
          value={username}
          type="text"
          autoComplete="off"
          id="input"
          placeholder="Your username.."
          onChange={(e) => setUsername(e.target.value)}
        ></input>
        {showError ? (
          <div className="error-message">Please enter your username.</div>
        ) : null}
        <button onClick={handleOnUsernameSubmit}>Start Playing!</button>
      </div>
    </ReactModal>
  );
};

export default Modal;
