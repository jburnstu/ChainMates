
import React, { useEffect, useState } from "react";
import { BrowserRouter, Link, Outlet, Route, Routes, useNavigate, useOutlet } from 'react-router-dom';

import { initialLoad, Login, Signup } from "./supportFuncs/authFuncs";
import { getArrayObjByID } from "./supportFuncs/utilityFuncs";

import { ModalSelectSegmentFromOptionsButton, StartNewStoryButton } from './buttons/workshopButtons.jsx';
import { WorkshopTab } from "./pages/workshopTab";

import { AuthorSearchButton, StorySearchButton } from "./buttons/searchButtons.jsx";
import { AuthorSearchPage } from "./pages/authorSearchPage";
import { StorySearchPage } from "./pages/storySearchPage";
import { contactAPI } from "./supportFuncs/utilityFuncs.jsx";

import { DashboardLayout, PageOrTabLayout } from "./layouts/layouts";


export default function App() {

    ///////  Load initial data or shunt into login/signup facade ////////////
    const [data, setData] = useState(null);
    const [authMode, setAuthMode] = useState("login"); 

    useEffect(() => {
        const initialLoad = async (callback) => {
            await contactAPI("load", "get", true)
                .then(function (value) {
                    callback(value);
                });
        }
        initialLoad(setData);
        }, []);

    if (!data) {
        return <div>Loading...</div>
    }

    if (Object.keys(data).length == 0) {
        switch (authMode) {
            case "login":
                return (
                    <Login
                        onLogin={setData}
                        switchToSignup={() => setAuthMode("signup")}
                    />
                );
            case "signup":
            default:
                return (
                    <Signup
                        onSignup={setData}
                        switchToLogin={() => setAuthMode("login")}
                    />
                );
        }
    }

    /////////////// This sectino uses the initially-loaded StartingURL to handle refreshes. /////////////////
    /////////////// It's a  bit outdated (from when the app was in Django), not sure it's doing  ///////////
    /////////////// quite what it needs to (not bugging at least though)           /////////////////////////
     
    const rootPath = "/chainmates/"; // not used right now
    const startingWriteOrReview = data.startingUrlDict.readOrWrite;
    let startingStoryID = data.startingUrlDict.storyId;

    let startingURL;
    if (startingWriteOrReview == null) {
        startingURL = "home";
    }
    else if (
        (startingWriteOrReview == "write" &&
            getArrayObjByID(data.writeDicts, startingStoryID) == undefined)
        ||
        (startingWriteOrReview == "review" &&
            getArrayObjByID(data.reviewDicts, startingStoryID) == undefined)
    ) {
        startingURL = `${startingWriteOrReview}`;
    }
    else {
        startingURL = `${startingWriteOrReview}/${startingStoryID}`;
        }


    ///////////// The function used to add / remove tabs when stories are loaded / submitted /////////////
    //////////// Passed down to various components (could be context perhaps?)             ///////////////

    async function changeStoryDicts(storyDict, writeOrReview = "write", addOrRemove = "add") {

        let dictArrayToChange = data[`${writeOrReview}Dicts`];
        let newDictArray;
        switch (addOrRemove) {
            case "remove":
                newDictArray = [];
                dictArrayToChange.forEach(originalStoryDict => {
                    if (originalStoryDict.id != storyDict.id) {
                        newDictArray.push(originalStoryDict);
                    }
                }
                )
                break
            case "add":
            default:
                newDictArray = dictArrayToChange.slice();
                newDictArray.push(storyDict);
        }
        if (newDictArray == dictArrayToChange) {
            console.warn("WARNING: ", storyDict, " was not successfully added to / removed from ", dictArrayToChange)
        }
        else { console.log("Successfully changed ", newDictArray, " to ", dictArrayToChange) };

        setData(data => ({ ...data, [`${writeOrReview}Dicts`]: newDictArray }));
        return newDictArray;
    }


    return ( //Could be a separate browserroutes document at some point
            <BrowserRouter>
                <Routes>
                    <Route path="" element={<UniversalHeader displayName={data.authorInfo.displayName} />}>
                        <Route path="" element={<RedirectToStartingURL startingURL={startingURL} />}
                            index/>
                        <Route path="home/" element={<HomeDashboard authorDict={data.authorInfo} />}
                        />
                        <Route path="write/" element={<WorkshopDashboard writeOrReview="write" dicts={data.writeDicts} setDicts={changeStoryDicts} />}
                        >
                            <Route path=":tabID/"
                                element={<WorkshopTab writeOrReview="write" dicts={data.writeDicts} setDicts={changeStoryDicts} />}
                            />
                        </Route>
                        <Route path="review/" element={<WorkshopDashboard writeOrReview="review" dicts={data.reviewDicts}
                            setDicts={changeStoryDicts} />}
                        >
                            <Route path=":tabID/"
                                element={<WorkshopTab writeOrReview="review" dicts={data.reviewDicts} setDicts={changeStoryDicts} />}
                            />
                        </Route>
                        <Route path="authors/" element={<SearchDashboard type="authors" />}>
                            <Route path=":authorID/" element={<AuthorSearchPage self={false} />} />
                        </Route>
                        <Route path="stories/" element={<SearchDashboard type="stories" />}>
                            <Route path=":storyID/" element={<StorySearchPage />}>
                                {/*<Route path=":finalSegmentID/" element={<StorySubSearchPage/>} />*/}
                            </Route>
                        </Route>
                        <Route path="settings/" element={<SettingsPage authorInfo={data.authorInfo} />}
                        />
                    </Route>
                    <Route path="*" element={<NoMatch />} />
                </Routes>
            </BrowserRouter>
    );
}

function UniversalHeader(props) {


    return (
        <div className="container">
            <header className="universalHeader">
                <h1>CHAIN MATES</h1>
                <h1>Hi, {props.displayName}!</h1>
                <nav>
                    <Link to="home" ><button type="button">HOME</button></Link>|{" "}
                    <Link to="write" ><button type="button">WRITE</button></Link>|{" "}
                    <Link to="review"><button type="button">READ</button></Link>|{" "}
                    <Link to="authors"><button type="button">AUTHORS</button></Link>|{" "}
                    <Link to="stories"><button type="button">STORIES</button></Link>|{" "}
                    <Link to="settings"><button type="button">SETTINGS</button></Link>
                </nav>
            </header >
            <Outlet />  {/*The rest of the app */}
        </div> 
    )
}



function RedirectToStartingURL(props) {
    // The index route, purely here to send you to any URL that's specified in the initial load
    let navigate = useNavigate();

    useEffect(() => {
        console.log("initital navigate")
        navigate(props.startingURL);
    }, [navigate, props.startingURL])

}

function HomeDashboard(props) {
    // Loads up your own AuthorDashboard (same as everyone else with a couple tweaks)
    return (
         <DashboardLayout
            leftSidebar={
                null
            }
            tabsList={
                null
            }
            pageOrTab={
                <AuthorSearchPage self={true} authorDict={props.authorDict} />
            }
        />
    )
}

function WorkshopDashboard(props) {

    let arrayOfTabIDs = props.dicts.map(dict => dict.id);

    const addNewTab = (tabID) => props.setDicts(tabID, props.writeOrReview, "add");
    const getTabName = (tabID, index) =>
        getArrayObjByID(props.dicts, tabID).storyData.title ?? `${index}.`;


    // Assign every outlet option its own "current content", so they can be saved in parallel
    let presavedCurrentContentByStory = {};
    props.dicts.forEach(dictInArray => {
        presavedCurrentContentByStory[dictInArray.id] =
            dictInArray.segmentHistoryList.slice(-1)[0].content;
    })
    const [currentContentByStory, setCurrentContentByStory] = useState(presavedCurrentContentByStory);
    const outlet = useOutlet([currentContentByStory, setCurrentContentByStory]);

    return (
        <DashboardLayout 
            leftSidebar={
                (props.writeOrReview == "write") //better as switch case?
                    ?
                    <>
                        <StartNewStoryButton addNewStory={addNewTab} />
                        <ModalSelectSegmentFromOptionsButton type="JOIN" addNewStory={addNewTab} />
                    </>
                    :
                        <ModalSelectSegmentFromOptionsButton type="MODERATE" addNewStory={addNewTab} />
            }
            tabsList={
                arrayOfTabIDs.map((tabID, index) =>
                    <Link to={tabID + "/"} key={index + tabID} className="tabLink">
                        <button className="tabButton">{getTabName(tabID, index)}
                        </button>
                    </Link>
                )
            }
            pageOrTab={
                outlet || <PageOrTabLayout/>
            }
        />
    )
}

function SearchDashboard(props) {
    const outlet = useOutlet();
    return (
         <DashboardLayout 
            leftSidebar={
                (props.type == "authors") // switch case?
                    ?
                    <AuthorSearchButton />
                    :
                    <StorySearchButton />
            }
            tabsList={
                null
            }
            pageOrTab={
                outlet || <PageOrTabLayout/>
            }
        />
    )
}

function SettingsPage(props) {
    const navigate = useNavigate();

    const [newDisplayName, setNewDisplayName] = useState("");
    const [newEmailAddress, setNewEmailAddress] = useState("");
    const [newPassword, setNewPassword] = useState("");

    function handleLogout() {
        localStorage.removeItem("token");
        sessionStorage.clear();
        navigate("/");
    }

    async function changeDisplayName(e) {
        await contactAPI("authors/", "patch", true, {
            displayName: newDisplayName
        });
    }
    async function changeEmailAddress(e) {
        await contactAPI("authors/", "patch", true, {
            emailAddress: newEmailAddress
        });
        handleLogout();
    }
    async function changePassword(e) {
        await contactAPI("authors/", "patch", true, {
            password: newPassword
        });
        handleLogout();
    }

    return (
        <fieldset>
            <label>Change Display Name
                <input label="Change Display Name" type="input"
                    value={newDisplayName}
                    onChange={(e) => setNewDisplayName(e.target.value)}>
                </input>
                <button type="submit" onClick={changeDisplayName} >{"->"}</button>
            </label>
            <label>Change Email Address
                <input label="Change Email Address" type="input"
                    value={newEmailAddress}
                    onChange={(e) => setNewEmailAddress(e.target.value)}>
                </input>
                <button type="submit" onClick={changeDisplayName} >{"->"}</button>
            </label>
            <label>Change Display Name
                <input label="Change Password" type="input"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}>
                </input>
                <button type="submit" onClick={changePassword} >{"->"}</button>
            </label>
            <button onClick={handleLogout}>LOGOUT</button>
        </fieldset>

    )
}


function NoMatch() {
    return (
        <div style={{ padding: 20 }}>
            <h2>404: Page Not Found</h2>
            <p>Lorem ipsum dolor sit amet, consectetur adip.</p>
        </div>
    );
}

