
import React, { StrictMode, useState, useEffect } from "react";
//import { createRoot } from 'react-dom/client';
import { BrowserRouter, Routes, Route, Link, Outlet, NavLink, useParams, useOutletContext, useOutlet, useNavigate } from 'react-router-dom';
import { SubmissionButton, ModalSelectSegmentFromOptionsButton, ModalNewButton } from './storyButtons.jsx';
import { AuthorContext } from "./context.jsx";
import { AuthorProfile, AuthorListDisplayButton } from "./authorProfileComponents.jsx";
import { Comments } from "./comments.jsx";
import { getArrayObjByID } from "./utilityFuncs";
import { Login, Signup } from "./authFuncs";
import { contactAPI } from "./utilityFuncs";


export default function App() {


    const [data, setData] = useState(null);
    const [authMode, setAuthMode] = useState("login"); 

    useEffect(() => {
        contactAPI("dashboardInfo", "get", true)
            .then(function (value) {
                console.log(value);
                setData(value)
            })
        }, []);

    if (!data) {
        return <div>Loading...</div>
    }

    console.log(data);
    if (Object.keys(data).length == 0) {
        console.log("IN HERE")
        switch (authMode) {
            case "login":
                console.log("GONIG TO LOGIN")
                return (
                    <Login
                        onLogin={setData}
                        switchToSignup={() => setAuthMode("signup")}
                    />
                );
                break;
            case "signup":
            default:
                return (
                    <Signup
                        onSignup={setData}
                        switchToLogin={() => setAuthMode("login")}
                    />
                );
                break;
    }

    const rootPath = "/chainmates/";
    const startingWriteOrReview = data.startingUrlDict.readOrWrite;
    let startingStoryID = data.startingUrlDict.storyId;

    let startingURL;
    if (startingWriteOrReview == null) {
        startingURL = "";
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
    // console.log(startingURL);


    async function changeStoryDicts(storyDict, writeOrReview = "write", addOrRemove = "add") {

        let dataKey;
        switch (writeOrReview) {
            case "write":
                dataKey = "writeDicts";
                break;
            case "review":
                dataKey = "reviewDicts";
                break;
            case "author":
                dataKey = "relationInfo";
                break;
        }

        let dictArrayToChange = data[dataKey];
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

        setData(data => ({ ...data, [dataKey]: newDictArray }));
        return newDictArray;
    }


    return (
            <BrowserRouter>
                <Routes>
                <Route path="" element={<UniversalHeader displayName={data.authorInfo.displayName} />}>
                        <Route path="" relative element={<Home startingURL={startingURL} />}
                            index />
                        <Route path="write/" element={<Dashboard writeOrReview="write" dicts={data.writeDicts} setDicts={changeStoryDicts} />}
                        >
                            <Route path=":storyID/"
                                element={<Story writeOrReview="write" dicts={data.writeDicts} setDicts={changeStoryDicts} />}
                            />
                        </Route>
                        <Route path="review/" element={<Dashboard writeOrReview="review" dicts={data.reviewDicts}
                            setDicts={changeStoryDicts} />}
                        >
                            <Route path=":storyID/"
                                element={<Story writeOrReview="review" dicts={data.reviewDicts} setDicts={changeStoryDicts} />}
                            />
                        </Route>
                        <Route path="author/"
                            element={<Dashboard writeOrReview="author" dicts={authorDicts} setDicts={changeStoryDicts}
                            />}
                        >
                            <Route path=":tabID/"
                                element={<AuthorProfile writeOrReview="author" dicts={authorDicts} setDicts={changeStoryDicts} />} />
                        </Route>
                    </Route>
                    <Route path="*" element={<NoMatch />} />
                </Routes>
            </BrowserRouter>
    );
}

function NoMatch() {
    return (
        <div style={{ padding: 20 }}>
            <h2>404: Page Not Found</h2>
            <p>Lorem ipsum dolor sit amet, consectetur adip.</p>
        </div>
    );
}

function Home(props) {

    let navigate = useNavigate();

    useEffect(() => {
        console.log("initital navigate")
        navigate(props.startingURL);
    }, [navigate,props.startingURL])

    return (<div className="container"></div>);
}


function UniversalHeader(props) {

    return (
        <>
            <header className="universalHeader">
                <h1>CHAIN MATES</h1>
                <h1>Hi, {props.displayName}!</h1>
                <nav>
                    <Link to="" ><button type="button">HOME</button></Link>|{" "}
                    <Link to="write" ><button type="button">WRITE</button></Link>|{" "}
                    <Link to="review"><button type="button">READ</button></Link>
                    <Link to="author"><button type="button">AUTHORS</button></Link>
                </nav>
            </header >
            <Outlet></Outlet>
        </>
    )
}



function Dashboard(props) {

    console.log(props.dicts)
    let arrayOfTabIDs = props.dicts.map(dict => dict.id);
    const addNewTab = (tabID) => props.setDicts(tabID, props.writeOrReview, "add");
    let getTabName;
    let presavedCurrentContentByStory = {};

    if (props.writeOrReview == "author") {
        console.log(props.dicts)
        getTabName = (id, index) =>
            getArrayObjByID(props.dicts, id).displayName ?? `${index}.`
    }
    else {
        getTabName = (id, index) =>
            getArrayObjByID(props.dicts, id).storyData.title ?? `${index}.`;

        props.dicts.forEach(dictInArray => {
            presavedCurrentContentByStory[dictInArray.id] =
                dictInArray.segmentHistoryList.slice(-1)[0].content;
        })

    }

    const [currentContentByStory, setCurrentContentByStory] = useState(presavedCurrentContentByStory);
    const outlet = useOutlet([currentContentByStory, setCurrentContentByStory]);

    return (
        <div className={props.writeOrReview + "DashboardContainer" + " dashboardContainer"}>
            <Sidebar writeOrReview={props.writeOrReview} addNewTab={addNewTab} />
            <nav className="tabs">
                {arrayOfTabIDs.map((tabID, index) =>
                    <Link to={tabID + "/"} key={index + tabID} className="tabLink">
                        <button className="tabButton">{getTabName(tabID, index)}</button>
                    </Link>
                )}
            </nav>
            {outlet || <PlaceHolder />}
        </div >
    )
}

function PlaceHolder() {
    return (<div className="storyContainer">PLACEHOLDER
        <div className="storyContent"></div>
        <div className="submissions"></div>
        <div className="comments"></div>
    </div>
    )
}

function Sidebar(props) {

    switch (props.writeOrReview) {
        case "author":
            return (
                <div className="sidebar">
                    <AuthorListDisplayButton addAuthorTab={props.addNewTab} />
                </div>
            )
        case "write":
            return (
                <div className="sidebar">
                    <ModalNewButton addNewStory={props.addNewTab} />
                    <ModalSelectSegmentFromOptionsButton type="JOIN" addNewStory={props.addNewTab} />
                </div>
            )
        case "review":
        default:
            return (
                <div className="sidebar">
                    <ModalSelectSegmentFromOptionsButton type="MODERATE" addNewStory={props.addNewTab} />
                </div>
            )
    }
}


function Story(props) {

    let writeOrReview = props.writeOrReview;
    const { storyID } = useParams();
    const [wordCount, setWordCount] = useState(0);
    const [currentContentByStory, setCurrentContentByStory] = useOutletContext();

    console.log(storyID, props.dicts);


    let storyDict = getArrayObjByID(props.dicts, storyID);
    let currentContent = currentContentByStory[storyID];

    let noSelections = {};
    storyDict.segmentHistoryList.forEach(dictInArray =>
        noSelections[dictInArray.id] = false)
    const [selectedSegmentDict, setSelectedSegmentDict] = useState(noSelections);

    function changeSegmentSelection(segmentID) {
        setSelectedSegmentDict({ ...selectedSegmentDict, [segmentID]: !selectedSegmentDict[segmentID] })
    }

    function handleChange(e) {
        setCurrentContentByStory({ ...currentContentByStory, [storyID]: e.target.value });
        setWordCount(getWordCount(currentContent));
    }

    function getWordCount(myText) {
        // const spaceMatchPattern = /[\w\d][\s\W*\d*]+[\w\d]/;
        const spaceMatchPattern = /\S+/g;
        let numberOfSpaces = myText.match(spaceMatchPattern);
        return (numberOfSpaces ? numberOfSpaces : []).length;
    }

    const removeCurrentStory = (storyDict) => props.setDicts(storyDict, writeOrReview, "remove");

    //storyDict.segmentHistoryList.foreach((segmentDict) => {
    //    console.log(segmentDict);
    //                    });

    return (
        <div className="storyContainer" id={"storyContainer" + { storyID }}>
            <StoryHeader storyDict={storyDict} wordCount={wordCount} />
            <div className="storyContent">
                {storyDict.segmentHistoryList.map(segmentDict =>
                    <SegmentDisplay key={segmentDict.id}
                        id={segmentDict.id}
                        isFinalSegment={segmentDict.id == storyID}
                        fixedContent={segmentDict.content}
                        currentContent={currentContent}
                        changeSelection={changeSegmentSelection}
                        onChange={handleChange} />
                )
                }
            </div>
            <SubmissionButtons writeOrReview={writeOrReview} currentContent={currentContent} segmentID={storyID} removeCurrentStory={removeCurrentStory} />
            <Comments selections={selectedSegmentDict} storyDict={storyDict} />
        </div>
    )
}

function StoryHeader(props) {

    let storyData = props.storyDict["storyData"];
    let length = props.storyDict.segmentHistoryList.length;

    return (<div className="storyHeader">
        <div>{storyData.title ? storyData.title : "Untitled"}</div>
        <div>{"Section : " + length + (storyData.maxNumberOfSegments ? " / " + storyData.maxSnumberOfSegments : "")}</div>
        <div>{"Word Count :" + props.wordCount + (storyData.maxSegmentLength ? " / " + storyData.maxSegmentLength : "")}</div>
    </div>)
}

function SegmentDisplay(props) {

    console.log(props.id)

    let readOnly = true;
    let onChange = null;
    let value = props.fixedContent;

    if (props.isFinalSegment) {
        readOnly = false;
        onChange = props.onChange;
        value = props.currentContent;
    }

    const onClick = () => {
        props.changeSelection(props.id)
    }


    return (
        <textarea className={`segmentDisplay ${readOnly ? undefined : 'currentSegmentDisplay'}`} readOnly={readOnly} value={value}
            onChange={onChange} onClick={onClick} ></ textarea>)

}


function SubmissionButtons(props) {

    let arrayOfButtonTypes;
    switch (props.writeOrReview) {
        case "review":
            arrayOfButtonTypes = ["APPROVE"];
            break;
        case "write":
        default:
            arrayOfButtonTypes = ["SAVE", "SUBMIT", "ABANDON"];
    }

    return (
        <div className="submissions">
            {arrayOfButtonTypes.map(buttonType =>
                <SubmissionButton
                    key={buttonType}
                    submissionType={buttonType}
                    currentContent={props.currentContent}
                    segmentID={props.segmentID}
                    removeCurrentStory={props.removeCurrentStory} />)}
        </div>
    )
}