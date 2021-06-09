package com.jetbrains.rider.plugins.verifytests

import com.intellij.diff.DiffContentFactory
import com.intellij.openapi.actionSystem.AnActionEvent
import com.jetbrains.rider.unitTesting.actions.base.RiderUnitTestAnActionBase
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTarget
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTargetExecutor
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTargetOperation
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTargetScope

class VerifyTestsAcceptReceivedAction :
    RiderUnitTestAnActionBase("UnitTestSession.VerifyTestsAcceptReceived") {
    override val target
        get() = RiderUnitTestTarget(
            RiderUnitTestTargetOperation.ElementManipulation,
            RiderUnitTestTargetExecutor.None,
            RiderUnitTestTargetScope.SelectedElements
        )

}