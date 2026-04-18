


export function DashboardLayout({ leftSidebar, tabsList, pageOrTab }) {
    return (
        <div className="dashboardContainer">
            <div className="leftSidebar">{leftSidebar}</div>
            <div className="tabsList">{tabsList}</div>
            <div className="pageOrTab">{pageOrTab}</div>
        </div>
    )
}

export function TabOrPageLayout({ topLine, mainContent, footer, rightSidebar }) {
    <div className="tabContainer">
        <div className="tabContainer"></div>
        <div className="topLine">{topLine}</div>
        <div className="mainContent">{mainContent}</div>
        <div className="footer">{footer}</div>
        <div className="rightSidebar">{rightSidebar}</div>
    </div>
}
