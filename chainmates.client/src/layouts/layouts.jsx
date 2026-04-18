


export function DashboardLayout({ leftSidebar, tabsList, pageOrTab }) {
    return (
        <div className="dashboardContainer">
            <div className="leftSidebar">{leftSidebar}</div>
            <div className="tabsList">{tabsList}</div>
            <div className="pageOrTabOuterContainer">{pageOrTab}</div>
        </div>
    )
}

export function PageOrTabLayout({ topLine, mainContent, footer, rightSidebar }) {
    return (
        <div className="pageOrTabInnerContainer">
            <div className="topLine">{topLine}</div>
            <div className="mainContent">{mainContent}</div>
            <div className="footer">{footer}</div>
            <div className="rightSidebar">{rightSidebar}</div>
        </div>
    )
}
