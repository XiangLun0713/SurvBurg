mergeInto(LibraryManager.library, {
    GetLeaderboard : function () {
        window.dispatchReactUnityEvent("GetLeaderboard");
    },

    UpdateUserHighScore : function (highscore) {
        window.dispatchReactUnityEvent("UpdateUserHighScore", highscore);
    },
});