package com.jetbrains.rider.plugins.verifytests

import com.intellij.diff.DiffContentFactory
import com.intellij.diff.DiffManager
import com.intellij.diff.contents.DiffContent
import com.intellij.diff.requests.SimpleDiffRequest
import com.intellij.diff.util.DiffUserDataKeys
import com.intellij.openapi.actionSystem.AnAction
import com.intellij.openapi.actionSystem.AnActionEvent
import com.intellij.openapi.vfs.VfsUtil
import com.intellij.openapi.vfs.VirtualFile
import com.intellij.util.ResourceUtil
import com.jetbrains.rider.unitTesting.actions.base.RiderUnitTestAnActionBase
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTarget
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTargetExecutor
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTargetOperation
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTargetScope
import icons.VerifyTestsIcons
import java.io.File
import javax.swing.Icon

class VerifyTestsCompareAction :
    RiderUnitTestAnActionBase("UnitTestSession.VerifyTestsCompare") {
    override val target
        get() = RiderUnitTestTarget(
            RiderUnitTestTargetOperation.ElementManipulation,
            RiderUnitTestTargetExecutor.None,
            RiderUnitTestTargetScope.SelectedElements
        )

}
//class VerifyTestsCompareAction :
//    AnAction(VerifyTestsIcons.CompareIcon)
//{
//    override fun actionPerformed(p0: AnActionEvent) {
//        val receivedFile = File("/Users/matt/RiderProjects/ClassLibrary2/ClassLibrary2/Class1.Test.received.txt")
//        val verifiedFile = File("/Users/matt/RiderProjects/ClassLibrary2/ClassLibrary2/Class1.Test.verified.txt")
//
//        val receivedVirtualFile = VfsUtil.findFileByIoFile(receivedFile, true)!!
//        val verifiedVirtualFile = VfsUtil.findFileByIoFile(verifiedFile, true)!!
//
//        val receivedContent = DiffContentFactory.getInstance().create(p0.project, receivedVirtualFile)
//        val verifiedContent = DiffContentFactory.getInstance().create(p0.project, verifiedVirtualFile)
//
//        receivedContent.putUserData(DiffUserDataKeys.FORCE_READ_ONLY, true)
//
//        val request = SimpleDiffRequest("Test Name", receivedContent, verifiedContent, "Received", "Verified");
//        DiffManager.getInstance().showDiff(p0.project, request);
//    }
//
//}