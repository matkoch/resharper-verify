package com.jetbrains.rider.plugins.verify

import com.intellij.diff.DiffContentFactory
import com.intellij.diff.DiffManager
import com.intellij.diff.requests.SimpleDiffRequest
import com.intellij.diff.util.DiffUserDataKeys
import com.intellij.openapi.project.Project
import com.intellij.openapi.vfs.VfsUtil
import com.jetbrains.rdclient.util.idea.LifetimedProjectComponent
import com.jetbrains.rider.projectView.solution
import java.io.File

class CompareManager(project: Project) : LifetimedProjectComponent(project) {
    init {
        project.solution.verifyTestsModel.compare.advise(componentLifetime) { compareData ->
            val receivedFile = File(compareData.receivedFile)
            val verifiedFile = File(compareData.verifiedFile)

            val receivedVirtualFile = VfsUtil.findFileByIoFile(receivedFile, true)!!
            val verifiedVirtualFile = VfsUtil.findFileByIoFile(verifiedFile, true)!!

            val receivedContent = DiffContentFactory.getInstance().create(project, receivedVirtualFile)
            val verifiedContent = DiffContentFactory.getInstance().create(project, verifiedVirtualFile)

            receivedContent.putUserData(DiffUserDataKeys.FORCE_READ_ONLY, true)

            val request = SimpleDiffRequest(compareData.testName, receivedContent, verifiedContent, "Received", "Verified")
            DiffManager.getInstance().showDiff(project, request)
        }
    }
}