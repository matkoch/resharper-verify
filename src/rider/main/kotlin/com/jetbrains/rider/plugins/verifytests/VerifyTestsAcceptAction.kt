package com.jetbrains.rider.plugins.verifytests

import com.jetbrains.rider.unitTesting.actions.base.RiderUnitTestAnActionBase
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTarget
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTargetExecutor
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTargetOperation
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTargetScope

class VerifyTestsAcceptAction :
    RiderUnitTestAnActionBase("UnitTestSession.VerifyTestsAccept") {
    override val target
        get() = RiderUnitTestTarget(
            RiderUnitTestTargetOperation.ElementManipulation,
            RiderUnitTestTargetExecutor.None,
            RiderUnitTestTargetScope.SelectedElements
        )

}

