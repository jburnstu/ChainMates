
import React, { useEffect, useState } from "react";
import { BrowserRouter, Link, Outlet, Route, Routes, useNavigate, useLocation, useOutlet, Navigate } from 'react-router-dom';

//import { initialLoad, Login, Signup } from "./supportFuncs/authFuncs";
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
    const [user, setUser] = useState(null);
    const [data, setData] = useState(null);
    const [authMode, setAuthMode] = useState("login"); 

    useEffect(() => {
        console.log("IN APP USEEFFECT");
        const initialLoad = async () => {
            let initialLoadData = await contactAPI("load", "get", true);
            setData(initialLoadData);
            setUser(initialLoadData.authorInfo);
        }
        if (data === null) {
            initialLoad();
        }

        });

    if (user === null) {
        return <div>Loading...</div>
    }

    async function handleLogout() {
        console.log("in app handle logout");
        await contactAPI("auth/logout/", "post", true);

        setUser(null);
        setData(null);
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
                <Route path="" element={<UniversalHeader displayName={user?.displayName} handleLogout={handleLogout} />} >
                    <Route index path="login/" element={<Login onLogin={setData}
                                                        />} />
                    <Route path="signup/" element={<Signup onSignup={setData}
                                    />} />
                    <Route path="home/" element={
                        <ProtectedRoute user={user} children={
                            <HomeDashboard authorDict={data?.authorInfo} />
                        } />} >
                    </Route>
                    <Route path="write/" element={
                        <ProtectedRoute user={user} children={
                            <WorkshopDashboard writeOrReview="write" dicts={data?.writeDicts} setDicts={changeStoryDicts} />
                        } />} >
                        <Route path=":tabID/"
                            element={<WorkshopTab writeOrReview="write" dicts={data?.writeDicts} setDicts={changeStoryDicts} />}
                            />
                    </Route>
                    <Route path="review/" element={
                        <ProtectedRoute user={user} children={
                            <WorkshopDashboard writeOrReview="review" dicts={data?.reviewDicts} setDicts={changeStoryDicts} />
                        } />} >
                        <Route path=":tabID/"
                            element={<WorkshopTab writeOrReview="review" dicts={data?.reviewDicts} setDicts={changeStoryDicts} />}
                            />
                    </Route>
                    <Route path="authors/" element={
                        <OptionalAuthRoute user={user} children={
                            <SearchDashboard type="authors" />
                        } />} >
                        <Route path=":authorID/"
                            element={<AuthorSearchPage />}
                        />
                    </Route>
                    <Route path="stories/" element={
                        <OptionalAuthRoute user={user} children={
                            <SearchDashboard type="stories" />
                        } />} >
                        <Route path=":storyID/"
                            element={<StorySearchPage />}
                        />
                    </Route>

                    <Route path="settings/" element={
                        <ProtectedRoute user={user} children={
                            <SettingsPage authorInfo={data?.authorInfo} handleLogout={handleLogout} />
                        } />} >
                    </Route>

                    </Route>
                <Route path="*" element={<NoMatch />} />
                </Routes>
            </BrowserRouter>
    );
}


function ProtectedRoute({ user, children }) {
    const location = useLocation();
    //const navigate = useNavigate();

    console.log("IN PROTECTED ROUTE CHECK" ,user);

    if (user === null) return <div>Loading...</div>;

    if (!user) {
        console.log("in !user branch")
        //navigate("/login");
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    return children;
}

function OptionalAuthRoute({ user, children }) {
    console.log("IN OPTIONAL ROUTE CHECK", user);
    if (user === null) return <div>Loading...</div>;
    return children;
}
function UniversalHeader(props) {
    const navigate = useNavigate();

    const handleLogout = () => {
        props.handleLogout();
        navigate("/login");
    }

    console.log("IN UNIVERSAL HEADER",props.displayName);
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
                    <Link to="settings"><button type="button">SETTINGS</button></Link>|{" "}
                    {props.displayName
                        ? <button type="button" onClick={handleLogout}>LOG OUT</button>
                        : <Link to="login"><button type="button">LOG IN</button></Link>       
                    }

                </nav>
            </header >
            <Outlet />  {/*The rest of the app */}
        </div> 
    )
}



export function Login({ onLogin, switchToSignup }) {
    console.log("IN LOGIN")
    const navigate = useNavigate();
    const location = useLocation();
    const [emailAddress, setEmailAddress] = useState("");
    const [password, setPassword] = useState("");


    const handleSubmit = async () => {
        await contactAPI("auth/login/", "post", true,
            // At some point want to generalise this to take username too
            { "EmailAddress": emailAddress, "Password": password });

        let initialLoadData = await contactAPI("load/", "get", true);
        onLogin(initialLoadData);

        const from = location.state?.from?.pathname || "/home";
        console.log(from);
        navigate(from, { replace: true });

    };


    return (
        <div>
            <input placeholder="email" onChange={e => setEmailAddress(e.target.value)} />
            <input type="password" onChange={e => setPassword(e.target.value)} />
            <button onClick={handleSubmit}>Login</button>

            <p onClick={() => navigate("/signup/")} style={{ cursor: "pointer" }}>
                Don't have an account? Sign up
            </p>
        </div>
    );
}


export function Signup({ onSignup, switchToLogin }) {
    const navigate = useNavigate();
    const location = useLocation();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [name, setName] = useState("");

    const handleSubmit = async () => {
        await contactAPI("auth/register", "post", true,
            { "EmailAddress": email, "Password": password, "DisplayName": name }
        )
        const dashboardInfoData = await contactAPI("load", "get", true);
        onSignup(dashboardInfoData);

        const from = location.state?.from?.pathname || "/home";
        navigate(from, { replace: true });
    };

    return (
        <div>
            <h2>Sign Up</h2>

            <input placeholder="name" onChange={e => setName(e.target.value)} />
            <input placeholder="email" onChange={e => setEmail(e.target.value)} />
            <input type="password" onChange={e => setPassword(e.target.value)} />

            <button onClick={handleSubmit}>Create Account</button>

            <p onClick={() => navigate("/login/")} style={{ cursor: "pointer" }}>
                Already have an account? Log in
            </p>
        </div>
    );
}
function HomeDashboard(props) {
    console.log("HOME");
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
    console.log(`IN ${props.writeOrReview} dashboard`);

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
    console.log(`in ${props.type} search page`)
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


    const handleLogout = () => {
        props.handleLogout();
        navigate("/login");
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
                <button type="submit" onClick={changeEmailAddress} >{"->"}</button>
            </label>
            <label>Change Password
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





/////////////// This sectino uses the initially-loaded StartingURL to handle refreshes. /////////////////
/////////////// It's a  bit outdated (from when the app was in Django), not sure it's doing  ///////////
/////////////// quite what it needs to (not bugging at least though)           /////////////////////////

//const rootPath = "/chainmates/"; // not used right now
//const startingWriteOrReview = data.startingUrlDict.readOrWrite;
//let startingStoryID = data.startingUrlDict.storyId;

//let startingURL;
//if (startingWriteOrReview == null) {
//    startingURL = "home";
//}
//else if (
//    (startingWriteOrReview == "write" &&
//        getArrayObjByID(data.writeDicts, startingStoryID) == undefined)
//    ||
//    (startingWriteOrReview == "review" &&
//        getArrayObjByID(data.reviewDicts, startingStoryID) == undefined)
//) {
//    startingURL = `${startingWriteOrReview}`;
//}
//else {
//    startingURL = `${startingWriteOrReview}/${startingStoryID}`;
//}



//<Route path="" element={<RedirectToStartingURL startingURL={startingURL} />}
//    index />



//if (Object.keys(data).length == 0) {
//    switch (authMode) {
//        case "login":
//            return (
//                <Login
//                    onLogin={setData}
//                    switchToSignup={() => setAuthMode("signup")}
//                />
//            );
//        case "signup":
//        default:
//            return (
//                <Signup
//                    onSignup={setData}
//                    switchToLogin={() => setAuthMode("login")}
//                />
//            );
//    }
//}


//function RedirectToStartingURL(props) {
//    // The index route, purely here to send you to any URL that's specified in the initial load
//    let navigate = useNavigate();

//    useEffect(() => {
//        console.log("initital navigate")
//        navigate(props.startingURL);
//    }, [navigate, props.startingURL])

//}